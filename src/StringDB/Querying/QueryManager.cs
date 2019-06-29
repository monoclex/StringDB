using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;
using System;
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
		private readonly bool _disposeDatabase;
		private readonly IMessageClient<QueryMessage<TKey, TValue>> _client;
		private readonly CancellationToken _cancellationToken;

		/// <summary>
		/// Creates a new query manager over a database.
		/// </summary>
		/// <param name="database">The database to use.</param>
		/// <param name="disposeDatabase">If the database should be disposed
		/// upon disposal of the QueryMaanger.</param>
		public QueryManager
		(
			IDatabase<TKey, TValue> database,
			bool disposeDatabase = true,
			CancellationToken cancellationToken = default
		)
		{
			_database = database;
			_disposeDatabase = disposeDatabase;
			_cancellationToken = cancellationToken;
			_client = new QueryManagerClient<TKey, TValue>(_database, _cancellationToken, _disposeDatabase);
		}

		public void Dispose()
		{
			_client.Dispose();

			// we depend on QueryManagerClient to do this for us
			// this is commented out in the event that the behaviour changes
			/*
			if (_disposeDatabase)
			{
				_database.Dispose();
			}
			*/
		}

		public async Task<bool> ExecuteQuery(IQuery<TKey, TValue> query)
		{
			// we expect it being disposed to not throw
			using (var proxy = new ProxiedClient<QueryMessage<TKey, TValue>>())
			using (var client = new LightweightClient<QueryMessage<TKey, TValue>>())
			using (var loadProxy = new LightweightClient<QueryMessage<TKey, TValue>>())
			{
				proxy.Proxy(client);

				client.Send(_client, new QueryMessage<TKey, TValue>
				{
					Go = true
				});

				var combined = CancellationTokenSource.CreateLinkedTokenSource(query.CancellationToken, _cancellationToken)
					.Token;

				// when we hear back a result, this will be our first message
				var result = await client.Receive(combined).ConfigureAwait(false);

				var initialId = result.Data.Id;

				// now we should constantly be receiving data from the database

				while (!query.CancellationToken.IsCancellationRequested)
				{
					bool requested = false;

					// TODO: have load proxy client
					var loadRequest = new LoadRequest<TKey, TValue>
					(
						result.Data.Id,
						result.Data.KeyValuePair.Value,
						query,
						() =>
						{
							requested = true;
							proxy.Proxy(loadProxy);
							return loadProxy;
						},
						_client,
						combined
					);

					var queryResult = await query.Process(result.Data.KeyValuePair.Key, loadRequest).ConfigureAwait(false);

					if (requested)
					{
						proxy.Deproxy(loadProxy);
						loadProxy.ClearQueue();
					}

					if (queryResult == QueryAcceptance.Completed)
					{
						Stop();
						return true;
					}

					// in the future, if QueryAcceptance gets more values, we should handle them.
					result = await client.Receive(combined).ConfigureAwait(false);

					// if we've looped back to the beginning
					if (result.Data.Id == initialId
						&& !result.Data.HasValue)
					{
						break;
					}
				}

				Stop();
				return false;

				void Stop()
				{
					client.Send(_client, new QueryMessage<TKey, TValue>
					{
						Stop = true
					});
				}
			}
		}

		public async Task ExecuteQuery(IWriteQuery<TKey, TValue> writeQuery)
		{
		}
	}
}