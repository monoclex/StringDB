using StringDB.Querying.Threading;

using System;
using System.Collections.Generic;
using System.Threading;

namespace StringDB.Querying
{
	public class CancellationTokenDisposable : IDisposable
	{
		public CancellationTokenDisposable(CancellationTokenSource cancellationTokenSource)
			=> CancellationTokenSource = cancellationTokenSource;

		public CancellationTokenSource CancellationTokenSource { get; }

		public void Dispose() => CancellationTokenSource.Cancel();
	}

	public static class DatabaseEnumerable
	{
		public static TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>> MakeTrainEnumerable<TKey, TValue>
		(
			this IDatabase<TKey, TValue> database,
			Func<ILazyLoader<TValue>, IRequest<TValue>> requestFactory,
			RequestLock @lock
		)
			=> new TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>>
			(
				database.EnumerateDatabaseLockibly
				(
					requestFactory,
					@lock
				)
			);

		public static IEnumerable<KeyValuePair<TKey, IRequest<TValue>>> EnumerateDatabaseLockibly<TKey, TValue>
		(
			this IDatabase<TKey, TValue> database,
			Func<ILazyLoader<TValue>, IRequest<TValue>> requestFactory,
			RequestLock @lock
		)
		{
			@lock.SemaphoreSlim.WaitAsync()
				.ConfigureAwait(false);

			foreach (var kvp in database)
			{
				var request = requestFactory(kvp.Value);

				@lock.SemaphoreSlim.Release();

				yield return new KeyValuePair<TKey, IRequest<TValue>>
					(
						kvp.Key,
						request
					);

				@lock.SemaphoreSlim.WaitAsync()
					.ConfigureAwait(false);
			}

			@lock.SemaphoreSlim.Release();
		}
	}
}