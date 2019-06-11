using JetBrains.Annotations;

using StringDB.Querying.Queries;
using StringDB.Querying.Threading;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	[PublicAPI]
	public class QueryManager<TKey, TValue> : IQueryManager<TKey, TValue>
	{
		private readonly IDatabase<TKey, TValue> _database;
		private readonly IQueryPool<TKey, TValue> _queryPool;
		private readonly ManualResetEventSlim _mres = new ManualResetEventSlim(false);
		private readonly RequestLock _databaseLock;
		private readonly Thread _thread;
		private int _current;

		public QueryManager
		(
			[NotNull] IDatabase<TKey, TValue> database,
			[CanBeNull] IQueryPool<TKey, TValue> queryPool = null,
			[CanBeNull] RequestLock databaseLock = null
		)
		{
			if (databaseLock == null)
			{
				databaseLock = new RequestLock(new SemaphoreSlim(1));
			}

			if (queryPool == null)
			{
				queryPool = new QueryPool<TKey, TValue>();
			}

			_database = database;
			_queryPool = queryPool;
			_databaseLock = databaseLock;

			_thread = new Thread(async () =>
			{
				while (true)
				{
					_current = 0;

					var released = false;

					// we will patiently wait to have the lock
					// before making any decisions about if the query is full or not.
					await _databaseLock.RequestAsync()
						.ConfigureAwait(false);

					try
					{
						// if there are no queries, wait for a signal.
						if ((await _queryPool.CurrentQueries()
							.ConfigureAwait(false))
							.Count == 0)
						{
							_mres.Reset();
							_databaseLock.Release();
							released = true;

							// wait for a signal to continue reading
							_mres.Wait();
							_mres.Reset();
						}
					}
					finally
					{
						if (!released)
						{
							_databaseLock.Release();
						}
					}

					var bc = new BlockingCollection<Task>();
					var cts = new CancellationTokenSource();
					var ct = cts.Token;
					var executionerPoolAwaiter = Task.Run(async () =>
					{
						try
						{
							while (!ct.IsCancellationRequested)
							{
								await bc.Take(ct)
									.ConfigureAwait(false);
							}
						}
						catch (OperationCanceledException)
						{
						}
					});

					await _databaseLock.SemaphoreSlim.WaitAsync()
						.ConfigureAwait(false);

					try
					{
						foreach (var kvp in _database)
						{
							var request = new SimpleDatabaseValueRequest<TValue>(kvp.Value, _databaseLock);

							var task = _queryPool.ExecuteQueries(_current, kvp.Key, request);
							bc.Add(task);

							_current++;

							// this will allow us to accept incoming queries
							// or read the values in the db

							await _databaseLock.AllowRequestsAsync()
								.ConfigureAwait(false);
						}
					}
					finally
					{
						_databaseLock.SemaphoreSlim.Release();
					}

					cts.Cancel();
					await executionerPoolAwaiter;

					while (bc.TryTake(out var task))
					{
						await task;
					}
				}
			});

			_thread.Start();
		}

		public void Dispose()
		{
			_thread.Abort();
		}

		public async Task<bool> ExecuteQuery([NotNull] IQuery<TKey, TValue> query)
		{
			var queryItem = new QueryItem<TKey, TValue>
			{
				CompletionSource = new TaskCompletionSource<bool>(),
				Query = query
			};

			// request the db lock so we can append this to the query
			await _databaseLock.RequestAsync()
				.ConfigureAwait(false);

			try
			{
				queryItem.Index = _current;
				await _queryPool.Append(queryItem)
					.ConfigureAwait(false);
			}
			finally
			{
				_databaseLock.Release();
			}

			// signals the thread to continue reading
			_mres.Set();

			return await queryItem.CompletionSource.Task;
		}

		public Task ExecuteQuery([NotNull] IWriteQuery<TKey, TValue> writeQuery) => throw new NotImplementedException();
	}
}