using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
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
		private TaskCompletionSource<bool> _added = new TaskCompletionSource<bool>(false);
		private bool _disposed = false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearQueue()
		{
			while (_queue.TryDequeue(out _))
			{
			}
		}

		public async Task<Message<TMessage>> Receive()
		{
			if (_disposed)
			{
				return Message<TMessage>.DefaultLackingData;
			}

			if (_queue.TryDequeue(out var result))
			{
				return result;
			}

			await _added.Task;

			if (_disposed)
			{
				return Message<TMessage>.DefaultLackingData;
			}

			if (!_queue.TryDequeue(out result))
			{
				throw new Exception("Expected added to be true if there were items in the queue.");
			}

			_added = new TaskCompletionSource<bool>(false);

			return result;
		}

		public void Queue(Message<TMessage> message)
		{
			if (_disposed)
			{
				return;
			}

			_queue.Enqueue(message);
			_added.SetResult(true);
		}

		public void Dispose()
		{
			_disposed = true;

			// we will pretend/except nothing to be called anymore
			_added.SetResult(true);

			ClearQueue();
		}
	}
}