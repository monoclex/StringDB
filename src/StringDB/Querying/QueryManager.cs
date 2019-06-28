using StringDB.Querying.Queries;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A query manager that uses a train enumerable for operations.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public class QueryManager<TKey, TValue> : IQueryManager<TKey, TValue>
	{
		private readonly IDatabase<TKey, TValue> _database;
		private readonly SemaphoreSlim _databaseLock;
		private readonly bool _disposeDatabase;

		private readonly ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
		private TaskCompletionSource<object> _writerTcs = new TaskCompletionSource<object>();
		private readonly SemaphoreSlim _writerLock = new SemaphoreSlim(1);
		private int _queryAmount = 0;

		/// <summary>
		/// Creates a new query manager over a database.
		/// </summary>
		/// <param name="database">The database to use.</param>
		/// <param name="disposeDatabase">If the database should be disposed
		/// upon disposal of the QueryMaanger.</param>
		public QueryManager
		(
			IDatabase<TKey, TValue> database,
			bool disposeDatabase = true
		)
			: this
		(
			database,
			new SemaphoreSlim(1),
			disposeDatabase
		)
		{
		}

		/// <summary>
		/// Creates a new query manager over a database with the lock speicfied.
		/// </summary>
		/// <param name="database">The database to use.</param>
		/// <param name="databaseLock">The lock on the database to apply.</param>
		/// <param name="disposeDatabase">If the database should be disposed
		/// upon disposal of the QueryMaanger.</param>
		public QueryManager
		(
			IDatabase<TKey, TValue> database,
			SemaphoreSlim databaseLock,
			bool disposeDatabase = true
		)
		{
			_database = database;
			_databaseLock = databaseLock;
			_disposeDatabase = disposeDatabase;
		}

		public void Dispose()
		{
			if (_disposeDatabase)
			{
				_database.Dispose();
			}
		}

		public async Task<bool> ExecuteQuery(IQuery<TKey, TValue> query)
		{
			await Task.Yield();

			_rw.EnterReadLock();

			try
			{
				return await _database
					.EnumerateWithLocking(_databaseLock)
					.ModifyValue(loader => (IRequest<TValue>) new SimpleDatabaseValueRequest<TValue>(loader, _databaseLock))
					.ForEachAsync<KeyValuePair<TKey, IRequest<TValue>>, bool>(async (element, controller) =>
				{
					if (query.IsCancellationRequested)
					{
						await Stop().ConfigureAwait(false);
						return;
					}

					var result = await query.Process(element.Key, element.Value).ConfigureAwait(false);

					if (result == QueryAcceptance.Completed)
					{
						controller.ProvideResult(true);
						await Stop().ConfigureAwait(false);
						return;
					}

					async Task Stop()
					{
						await _writerLock.WaitAsync()
							.ConfigureAwait(false);

						try
						{
							_queryAmount--;

							if (_queryAmount == 0)
							{
								_writerTcs.SetResult(true);
							}
						}
						finally
						{
							_writerLock.Release();
						}
					}
				}).ConfigureAwait(false);
			}
			finally
			{
				_rw.ExitReadLock();
			}
		}

		public async Task ExecuteQuery(IWriteQuery<TKey, TValue> writeQuery)
		{
			await _databaseLock.WaitAsync()
				.ConfigureAwait(false);

			_rw.EnterWriteLock();

			try
			{
				writeQuery.Execute(_database);
			}
			finally
			{
				_rw.ExitWriteLock();
				_databaseLock.Release();
			}
		}
	}
}