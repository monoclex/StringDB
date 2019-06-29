using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Uses a message client to load a value from a database.
	/// </summary>
	public struct LoadRequest<TKey, TValue> : IRequest<TValue>
	{
		private readonly int _id;
		private readonly Func<IMessageClient<QueryMessage<TKey, TValue>>> _clientFactory;
		private readonly IMessageClient<QueryMessage<TKey, TValue>> _queryManager;
		private readonly CancellationToken _cancellationToken;
		private readonly IQuery<TKey, TValue> _query;
		private readonly ILazyLoader<TValue> _loader;

		public LoadRequest
		(
			int id,
			ILazyLoader<TValue> loader,
			IQuery<TKey, TValue> query,
			Func<IMessageClient<QueryMessage<TKey, TValue>>> clientFactory,
			IMessageClient<QueryMessage<TKey, TValue>> queryManager,
			CancellationToken cancellationToken
		)
		{
			_id = id;
			_clientFactory = clientFactory;
			_queryManager = queryManager;
			_cancellationToken = cancellationToken;
			_query = query;
			_loader = loader;
		}

		public async Task<TValue> Request()
		{
			var client = _clientFactory();

			client.Send(_queryManager, new QueryMessage<TKey, TValue>
			{
				KeyValuePair = new KeyValuePair<TKey, ILazyLoader<TValue>>(default, _loader),
				Id = _id,

				// we want to LOAD a value
				HasValue = true
			});

			do
			{
				// TODO: ct here?
				var result = await client.Receive(_cancellationToken).ConfigureAwait(false);

				if (result.Data.HasValue
					&& result.Data.Id == _id
					&& !_cancellationToken.IsCancellationRequested) // this last one will exit the loop if a cancellation was requested
				{
					return result.Data.Value;
				}
			} while (!_cancellationToken.IsCancellationRequested);

			throw new TaskCanceledException();
		}

		public void Dispose()
		{
		}
	}
}
