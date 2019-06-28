using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public struct QueryMessage
	{
		public int Id;
		public bool Stop;
		public bool Go;
	}

	/// <summary>
	/// A query manager that uses a train enumerable for operations.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public class QueryManager<TKey, TValue> : IQueryManager<TKey, TValue>
	{
		private readonly IDatabase<TKey, TValue> _database;
		private readonly bool _disposeDatabase;
		private readonly IMessageClient<QueryMessage> _client;

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
		{
			_database = database;
			_disposeDatabase = disposeDatabase;
			_client = new ManagedClient<QueryMessage>
			(
				async (client, cancellatonToken) =>
				{
					// we would wait for messages on one task while enumerating on the other
					// and prepare to give clients access to messages and such
					var message = await client.Receive().ConfigureAwait(false);
				}
			);
		}

		public void Dispose()
		{
			_client.Dispose();

			if (_disposeDatabase)
			{
				_database.Dispose();
			}
		}

		public async Task<bool> ExecuteQuery(IQuery<TKey, TValue> query)
		{
			return false;
		}

		public async Task ExecuteQuery(IWriteQuery<TKey, TValue> writeQuery)
		{
		}
	}
}