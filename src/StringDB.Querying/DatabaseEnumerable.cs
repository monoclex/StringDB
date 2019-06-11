using StringDB.Querying.Threading;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StringDB.Querying
{
	public static class DatabaseEnumerable
	{
		public static IAsyncEnumerable<KeyValuePair<TKey, IRequest<TValue>>> Enumerate<TKey, TValue>
		(
			this IDatabase<TKey, TValue> database,
			Func<ILazyLoader<TValue>, IRequest<TValue>> requestFactory,
			RequestLock @lock
		)
			=> new AsyncEnumerable<KeyValuePair<TKey, IRequest<TValue>>>(async yield =>
			{
				await @lock.SemaphoreSlim.WaitAsync()
					.ConfigureAwait(false);

				foreach (var kvp in database)
				{
					var request = requestFactory(kvp.Value);

					using (var cts = new CancellationTokenSource())
					{
						var lazyUnloadTask = @lock.LazyReleaseAsync(cts.Token);

						await yield.ReturnAsync(new KeyValuePair<TKey, IRequest<TValue>>
						(
							kvp.Key,
							request
						)).ConfigureAwait(false);

						cts.Cancel();
						await lazyUnloadTask;
					}
				}

				@lock.SemaphoreSlim.Release();
			});
	}
}
