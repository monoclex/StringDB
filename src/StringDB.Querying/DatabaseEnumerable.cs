using StringDB.Querying.Threading;

using System;
using System.Collections.Generic;
using System.Threading;

namespace StringDB.Querying
{
	public static class DatabaseEnumerable
	{
		public static TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>> MakeTrainEnumerable<TKey, TValue>
		(
			this IDatabase<TKey, TValue> database,
			Func<ILazyLoader<TValue>, IRequest<TValue>> requestFactory,
			SemaphoreSlim @lock
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
			SemaphoreSlim @lock
		)
		{
			@lock.WaitAsync()
				.ConfigureAwait(false);

			foreach (var kvp in database)
			{
				var request = requestFactory(kvp.Value);

				@lock.Release();

				yield return new KeyValuePair<TKey, IRequest<TValue>>
					(
						kvp.Key,
						request
					);

				@lock.WaitAsync()
					.ConfigureAwait(false);
			}

			@lock.Release();
		}
	}
}