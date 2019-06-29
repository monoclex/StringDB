using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Uses a message client to load a value from a database.
	/// </summary>
	public struct LoadRequest<TKey, TValue> : IRequest<TValue>
	{
		private readonly int _id;
		private readonly IMessageClient<QueryMessage<TKey, TValue>> _client;
		private readonly IMessageClient<QueryMessage<TKey, TValue>> _queryManager;
		private readonly IQuery<TKey, TValue> _query;
		private readonly ILazyLoader<TValue> _loader;

		public LoadRequest
		(
			int id,
			ILazyLoader<TValue> loader,
			IQuery<TKey, TValue> query,
			IMessageClient<QueryMessage<TKey, TValue>> client,
			IMessageClient<QueryMessage<TKey, TValue>> queryManager
		)
		{
			_id = id;
			_client = client;
			_queryManager = queryManager;
			_query = query;
			_loader = loader;
		}

		public async Task<TValue> Request()
		{
			_client.Send(_queryManager, new QueryMessage<TKey, TValue>
			{
				KeyValuePair = new KeyValuePair<TKey, ILazyLoader<TValue>>(default, _loader),
				Id = _id,

				// we want to LOAD a value
				HasValue = true
			});

			do
			{
				// TODO: ct here?
				var result = await _client.Receive().ConfigureAwait(false);

				if (result.Data.HasValue
					&& result.Data.Id == _id
					&& !_query.IsCancellationRequested) // this last one will exit the loop if a cancellation was requested
				{
					return result.Data.Value;
				}
			} while (!_query.IsCancellationRequested);

			throw new TaskCanceledException();
		}

		public void Dispose()
		{
		}
	}
}
