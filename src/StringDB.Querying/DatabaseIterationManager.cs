using JetBrains.Annotations;

using StringDB.Querying.Messaging;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	// TODO: test *later* since i'm not sure if this is the full implementation we'll need

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
					var request = await _requestManager.NextRequest(cts);

					cts.ThrowIfCancellationRequested();

					TValue value;

					lock (_lock)
					{
						value = request.RequestKey.Load();
					}

					request.SupplyValue(value);
				}
			});
		}

		public async ValueTask IterateTo
		(
			IMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> target,
			CancellationToken cancellationToken = default
		)
		{
			await Task.Yield();

			foreach (var kvp in _database.LockWhenEnumerating(_lock))
			{
				var key = kvp.Key;
				var value = kvp.Value;

				var request = _requestManager.CreateRequest(value);

				target.Enqueue(new KeyValuePair<TKey, IRequest<TValue>>(key, request));
			}
		}

		public void Dispose()
		{
			_requestHandler.Dispose();
		}
	}
}