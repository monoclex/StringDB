using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public class QueryPool<TKey, TValue> : IQueryPool<TKey, TValue>
	{
		private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
		private readonly List<QueryItem<TKey, TValue>> _pool = new List<QueryItem<TKey, TValue>>();

		public async Task<IReadOnlyCollection<QueryItem<TKey, TValue>>> CurrentQueries()
		{
			await _lock.WaitAsync()
				.ConfigureAwait(false);

			try
			{
				return _pool.ToArray();
			}
			finally
			{
				_lock.Release();
			}
		}

		public async Task Append(QueryItem<TKey, TValue> queryItem)
		{
			await _lock.WaitAsync()
				.ConfigureAwait(false);

			try
			{
				_pool.Add(queryItem);
			}
			finally
			{
				_lock.Release();
			}
		}

		public async Task ExecuteQueries(int index, TKey key, IRequest<TValue> value)
		{
			var queries = await CurrentQueries()
				.ConfigureAwait(false);

			var needsAwaiting = new List<Task>(queries.Count);
			var needsKilling = new List<QueryItem<TKey, TValue>>(queries.Count);

			var i = 0;
			foreach (var query in queries)
			{
				// first, check if it's looped around
				if (index == query.Index)
				{
					if (query.JustInserted)
					{
						query.JustInserted = false;
						goto @add;
					}

					// kill it
					query.CompletionSource.SetResult(false);
					needsKilling.Add(query);

					goto @continue;
				}

			@add:
				// then execute it
				needsAwaiting.Add
				(
					ExecuteQuery
					(
						query,
						key,
						value,
						needsKilling
					)
				);

			@continue:
				i++;
			}

			await Task.WhenAll(needsAwaiting)
				.ConfigureAwait(false);

			// now we need to kill the tasks that need to die

			await _lock.WaitAsync()
				.ConfigureAwait(false);

			try
			{
				var indexes = new List<int>();
				foreach (var kill in needsKilling)
				{
					var killIndex = needsKilling.IndexOf(kill);

					if (killIndex != -1)
					{
						indexes.Insert(0, killIndex);
					}
				}

				foreach (var killIndex in indexes)
				{
					_pool.RemoveAt(killIndex);
				}
			}
			finally
			{
				_lock.Release();
			}
		}

		private async Task ExecuteQuery
		(
			QueryItem<TKey, TValue> queryItem,
			TKey key,
			IRequest<TValue> value,
			List<QueryItem<TKey, TValue>> needsKilling
		)
		{
			var query = queryItem.Query;

			var queryAcceptance = await query.Accept(key, value)
				.ConfigureAwait(false);

			switch (queryAcceptance)
			{
				case QueryAcceptance.NotAccepted: return;

				case QueryAcceptance.Accepted:
				{
					// only process :(
					await query.Process(key, value)
						.ConfigureAwait(false);
				}
				return;

				case QueryAcceptance.Completed:
				{
					await query.Process(key, value)
						.ConfigureAwait(false);

					// we have permission to kill the query since
					// it's finished now
					queryItem.CompletionSource.SetResult(true);

					needsKilling.Add(queryItem);
				}
				return;
			}
		}

		// we are expecting the caller to know that there's no tasks running
		public void Dispose()
		{
			_lock.Dispose();
			_pool.Clear();
		}
	}
}