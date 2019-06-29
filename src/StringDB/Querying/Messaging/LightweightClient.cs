using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	/// <summary>
	/// This client is lightweight because there is no thread
	/// doing any kind of processing at all times. It is entirely
	/// up to the user of this class to manage when a message
	/// should be received.
	/// </summary>
	public class LightweightClient<TMessage> : IMessageClient<TMessage>
	{
		private readonly ConcurrentQueue<Message<TMessage>> _queue = new ConcurrentQueue<Message<TMessage>>();
		private TaskCompletionSource<bool> _disposedTask = new TaskCompletionSource<bool>();
		private bool _disposed = false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearQueue()
		{
			while (_queue.TryDequeue(out _))
			{
			}
		}

		public async Task<Message<TMessage>> Receive(CancellationToken cancellationToken)
		{
			if (_disposed)
			{
				return Message<TMessage>.DefaultLackingData;
			}

			var dequeue = TryDequeue();

			if (dequeue.IsCompleted)
			{
				return await dequeue;
			}

			var tcs = new TaskCompletionSource<bool>();
			using (var registration = cancellationToken.Register(() =>
			{
				if (tcs == null)
				{
					return;
				}

				tcs.SetResult(true);
			}, false))
			{
				await Task.WhenAny(_disposedTask.Task, dequeue, tcs.Task).ConfigureAwait(false);
			}
			tcs = null;

			if (_disposed)
			{
				return Message<TMessage>.DefaultLackingData;
			}

			return await dequeue;
		}

		private async Task<Message<TMessage>> TryDequeue()
		{
			await Task.Yield();

			while (!_disposed)
			{
				if (_queue.TryDequeue(out var result))
				{
					return result;
				}
			}

			return Message<TMessage>.DefaultLackingData;
		}

		public void Queue(Message<TMessage> message)
		{
			if (_disposed)
			{
				return;
			}

			_queue.Enqueue(message);
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			_disposedTask.SetResult(true);

			ClearQueue();
		}
	}
}