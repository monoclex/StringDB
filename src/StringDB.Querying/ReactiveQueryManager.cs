using JetBrains.Annotations;

using StringDB.Querying.Queries;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public class ReactiveQueryManager<TKey, TValue> : IQueryManager<TKey, TValue>
	{
		private readonly TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>> _trainEnumerable;

		public ReactiveQueryManager(TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>> trainEnumerable)
		{
			_trainEnumerable = trainEnumerable;
		}

		public void Dispose() => throw new NotImplementedException();

		public async Task<bool> ExecuteQuery([NotNull] IQuery<TKey, TValue> query)
		{
			await Task.Yield();

			return await _trainEnumerable.ForEachAsync<KeyValuePair<TKey, IRequest<TValue>>, bool>(async (element, controller) =>
			{
				if (query.IsCancellationRequested)
				{
					Console.WriteLine("query is cancellation requested");
					controller.Stop();
					return;
				}

				var result = await query.Process(element.Key, element.Value).ConfigureAwait(false);

				if (result == QueryAcceptance.Completed)
				{
					Console.WriteLine("query is completed");
					controller.ProvideResult(true);
					controller.Stop();
					return;
				}
			}).ConfigureAwait(false);
		}

		public Task ExecuteQuery([NotNull] IWriteQuery<TKey, TValue> writeQuery) => throw new NotImplementedException();
	}
}