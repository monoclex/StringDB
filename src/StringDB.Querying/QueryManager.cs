using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;

namespace StringDB.Querying
{
	[PublicAPI]
	public sealed class QueryManager<TKey, TValue> : IQueryManager<TKey, TValue>
	{
		private readonly QueryManagerExecutioner<TKey, TValue> _executioner;

		public QueryManager
		(
			QueryManagerExecutioner<TKey, TValue> executioner
		)
		{
			_executioner = executioner;
		}

		public async ValueTask<bool> ExecuteQuery([NotNull] IQuery<TKey, TValue> query)
		{
			using (var pipe = new ChannelMessagePipe<KeyValuePair<TKey, IRequest<TValue>>>())
			{
				_executioner.Attach(pipe, out var index);
				var i = index;

				while (!query.CancellationToken.IsCancellationRequested)
				{
					var kvp = await pipe.Dequeue(query.CancellationToken).ConfigureAwait(false);

					if (query.CancellationToken.IsCancellationRequested)
					{
						return false;
					}

					var result = await query.Process(kvp.Key, kvp.Value).ConfigureAwait(false);

					if (result == QueryAcceptance.Completed)
					{
						return true;
					}

					i++;
				}

				return false;
			}
		}

		public ValueTask ExecuteQuery([NotNull] IWriteQuery<TKey, TValue> writeQuery) => throw new NotImplementedException();

		public void Dispose() => throw new NotImplementedException();
	}
}
