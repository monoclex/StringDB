using JetBrains.Annotations;

using StringDB.Querying.Messaging;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// The most basic implementation of a <see cref="IIterationManager{TKey, TValue}"/>,
	/// that aims to implement an <see cref="IIterationManager{TKey, TValue}"/> by
	/// iterating over an <see cref="IDatabase{TKey, TValue}"/>.
	/// </summary>
	[PublicAPI]
	public sealed class DatabaseIterationManager<TKey, TValue> : IIterationManager<TKey, TValue>
	{
		[NotNull] private readonly object _lock = new object();
		[NotNull] private readonly IDatabase<TKey, TValue> _database;
		[NotNull] private readonly IRequestManager<ILazyLoader<TValue>, TValue> _requestManager;
		[NotNull] private readonly BackgroundTask _requestHandler;

		public DatabaseIterationManager
		(
			[NotNull] IDatabase<TKey, TValue> database,
			[NotNull] IRequestManager<ILazyLoader<TValue>, TValue> requestManager
		)
		{
			_database = database;
			_requestManager = requestManager;

			_requestHandler = new BackgroundTask(async (cts) =>
			{
				while (!cts.IsCancellationRequested)
				{
					var request = await _requestManager.NextRequest(cts).ConfigureAwait(false);

					if (cts.IsCancellationRequested)
					{
						// unfortunately we cannot fulfill the next request
						// because it probably quit early
						return;
					}

					TValue value;

					lock (_lock)
					{
						value = request.RequestKey.Load();
					}

					request.SupplyValue(value);

					if (cts.IsCancellationRequested)
					{
						return;
					}
				}
			});
		}

		[NotNull]
		public IIterationHandle IterateTo
		(
			[NotNull] IMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> target,
			CancellationToken cancellationToken = default
		)
		{
			var index = 0;
			var atEnd = false;

			return new IterationHandle
			(
				new BackgroundTask(async (cts) =>
				{
					await Task.Yield();

					foreach (var item in _database.LockWhenEnumerating(_lock))
					{
						var key = item.Key;
						var value = _requestManager.CreateRequest(item.Value);

						target.Enqueue(new KeyValuePair<TKey, IRequest<TValue>>(key, value));

						if (cts.IsCancellationRequested)
						{
							break;
						}

						index++;
					}

					atEnd = true;
				}),
				() => index,
				() => atEnd
			);
		}

		public void Dispose() => _requestHandler.Dispose();
	}
}