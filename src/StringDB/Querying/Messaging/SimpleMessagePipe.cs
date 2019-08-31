using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	/// <summary>
	/// A simple implementation of the MessagePipe.
	/// Expects there to only be one dequeueing task/thread.
	/// </summary>
	/// <typeparam name="T">The type of item to transport.</typeparam>
	public class SimpleMessagePipe<T> : IMessagePipe<T>
	{
		private readonly ManualResetEventSlim _mres = new ManualResetEventSlim(false);
		private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

		public void Dispose()
		{
			_mres.Dispose();
		}

		public void Enqueue(T message)
		{
			_queue.Enqueue(message);
			_mres.Set();
		}

		public Task<T> Dequeue(CancellationToken cancellationToken = default)
		{
			if (_queue.TryDequeue(out var result))
			{
				return Task.FromResult(result);
			}

			return AsyncDequeue(cancellationToken);
		}

		// https://stackoverflow.com/a/18766131
		private Task<T> AsyncDequeue(CancellationToken cancellationToken)
		{
			var tcs = new TaskCompletionSource<T>();

			var registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_mres.WaitHandle, (state, timedOut) =>
			{
				if (!_queue.TryDequeue(out var result))
				{
					tcs.SetException(new Exception("Multiple async dequeuers? Unable to dequeue."));
					return;
				}

				tcs.SetResult(result);
			}, tcs, -1, true);

			tcs.Task.ContinueWith((_, state) =>
			{
				var localRegisteredWaitHandle = (RegisteredWaitHandle)state;

				localRegisteredWaitHandle.Unregister(null);
			}, registeredWaitHandle, cancellationToken);

			return tcs.Task;
		}
	}
}