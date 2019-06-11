using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Threading
{
	/// <summary>
	/// A wrapper over a semaphore slim that allows only one
	/// thread to access something at a time, but allows a
	/// master thread to maintain the lock for as long as
	/// possible until it feels like giving it up.
	/// </summary>
	public class RequestLock
	{
		public RequestLock(SemaphoreSlim semaphoreSlim)
			=> SemaphoreSlim = semaphoreSlim;

		public SemaphoreSlim SemaphoreSlim { get; }
		private bool _released;
		public int AccessRequests;

		/// <summary>
		/// Used by the caller. It adds a request so the
		/// master thread knows to give up its position of
		/// power.
		/// </summary>
		public Task RequestAsync()
		{
			Interlocked.Increment(ref AccessRequests);
			return SemaphoreSlim.WaitAsync();
		}

		/// <summary>
		/// Used by the caller. If it has a lock on it, it
		/// should release it here.
		/// </summary>
		public void Release()
		{
			Interlocked.Decrement(ref AccessRequests);
			SemaphoreSlim.Release();
		}

		public async Task LazyReleaseAsync(CancellationToken cancellationToken = default)
		{
			await Task.Yield();

			var spinLock = new SpinWait();

			while (!cancellationToken.IsCancellationRequested)
			{
				spinLock.SpinOnce();

				if (AccessRequests > 0)
				{
					await AllowRequestsAsync()
						.ConfigureAwait(false);
				}
			}
		}

		/// <summary>
		/// Used by the master thread. This allows any callers
		/// who requst the lock to have it.
		/// </summary>
		public async Task AllowRequestsAsync()
		{
			if (AccessRequests == 0)
			{
				return;
			}

			SemaphoreSlim.Release();

			var spinWait = new SpinWait();

			while (AccessRequests > 0)
			{
				spinWait.SpinOnce();
			}

			await SemaphoreSlim.WaitAsync()
				.ConfigureAwait(false);
		}
	}
}
