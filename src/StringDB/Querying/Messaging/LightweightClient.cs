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
		private readonly BlockingCollection<Message<TMessage>> _queue = new BlockingCollection<Message<TMessage>>();
		private CancellationTokenSource _disposeCts = new CancellationTokenSource();
		private bool _disposed = false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearQueue()
		{
			while (_queue.TryTake(out _))
			{
			}
		}

		public async Task<Message<TMessage>> Receive(CancellationToken cancellationToken)
		{
			if (_disposed)
			{
				return Message<TMessage>.DefaultLackingData;
			}

			if (_queue.TryTake(out var dequeueResult))
			{
				return dequeueResult;
			}

			await Task.Yield();

			var combined = CancellationTokenSource.CreateLinkedTokenSource
			(
				cancellationToken,
				_disposeCts.Token
			);

			try
			{
				var result = _queue.Take(combined.Token);

				if (_disposed)
				{
					return Message<TMessage>.DefaultLackingData;
				}

				return result;
			}
			catch (OperationCanceledException)
			{
				return Message<TMessage>.DefaultLackingData;
			}
		}

		public void Queue(Message<TMessage> message)
		{
			if (_disposed)
			{
				return;
			}

			_queue.Add(message);
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			_disposeCts.Cancel();

			ClearQueue();
		}
	}
}