using JetBrains.Annotations;
using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Used by a query manager to represent the state of a query.
	/// This will manage itself and communicates with the query manager over
	/// message pipes, telling the query manager what it needs.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public sealed class QueryState<TKey, TValue> : IDisposable
	{
		[NotNull] private readonly IQuery<TKey, TValue> _query;
		[NotNull] private readonly IMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> _consumePipe;

		public QueryState
		(
			[NotNull] IQuery<TKey, TValue> query,
			[NotNull] IMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> consumePipe
		)
		{
			_query = query;
			_consumePipe = consumePipe;
		}

		[NotNull]
		public async Task Run()
		{
			while (!_query.CancellationToken.IsCancellationRequested)
			{
				var kvp = await _consumePipe.Dequeue(_query.CancellationToken).ConfigureAwait(false);

				var acceptance = await _query.Process(kvp.Key, kvp.Value).ConfigureAwait(false);

				if (acceptance == QueryAcceptance.Completed)
				{
					return;
				}
			}
		}

		public void Dispose()
		{
			_consumePipe.Dispose();
			_query.Dispose();
		}
	}
}