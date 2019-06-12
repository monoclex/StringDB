using StringDB.Querying.Queries;

using System.Collections.Generic;
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
		private readonly TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>> _trainEnumerable;
		private readonly IDatabase<TKey, TValue> _database;
		private readonly SemaphoreSlim _databaseLock;
		private readonly bool _disposeDatabase;

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

			_trainEnumerable = _database.MakeTrainEnumerable
			(
				value => new SimpleDatabaseValueRequest<TValue>(value, _databaseLock),
				databaseLock
			);
		}

		public void Dispose()
		{
			_trainEnumerable.Dispose();

			if (_disposeDatabase)
			{
				_database.Dispose();
			}
		}

		public async Task<bool> ExecuteQuery(IQuery<TKey, TValue> query)
		{
			await Task.Yield();

			return await _trainEnumerable.ForEachAsync<KeyValuePair<TKey, IRequest<TValue>>, bool>(async (element, controller) =>
			{
				if (query.IsCancellationRequested)
				{
					controller.Stop();
					return;
				}

				var result = await query.Process(element.Key, element.Value).ConfigureAwait(false);

				if (result == QueryAcceptance.Completed)
				{
					controller.ProvideResult(true);
					controller.Stop();
					return;
				}
			}).ConfigureAwait(false);
		}

		public async Task ExecuteQuery(IWriteQuery<TKey, TValue> writeQuery)
		{
			await _databaseLock.WaitAsync()
				.ConfigureAwait(false);

			writeQuery.Execute(_database);

			_databaseLock.Release();
		}
	}
}