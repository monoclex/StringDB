using JetBrains.Annotations;

using StringDB.Querying.Queries;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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

			var process = new ConcurrentQueue<KeyValuePair<TKey, IRequest<TValue>>>();

			{
				bool finished = false;

				var cts = new CancellationTokenSource();
				var consumeTask = Task.Run<bool>(async () =>
				{
					try
					{
						while (!cts.IsCancellationRequested)
						{
							if (!process.TryDequeue(out var kvp))
							{
								continue;
							}

							if (await WorkOn(kvp).ConfigureAwait(false))
							{
								finished = true;
								return true;
							}
						}
					}
					catch (OperationCanceledException)
					{
					}

					while (process.TryDequeue(out var kvp))
					{
						if (await WorkOn(kvp).ConfigureAwait(false))
						{
							finished = true;
							return true;
						}
					}

					return false;

					async Task<bool> WorkOn(KeyValuePair<TKey, IRequest<TValue>> kvp)
					{
						var result = await query.Accept(kvp.Key, kvp.Value).ConfigureAwait(false);

						if (result == QueryAcceptance.Completed)
						{
							await query.Process(kvp.Key, kvp.Value).ConfigureAwait(false);
							return true;
						}

						if (result == QueryAcceptance.Completed)
						{
							await query.Process(kvp.Key, kvp.Value).ConfigureAwait(false);
						}

						return false;
					}
				});

				foreach (var item in _trainEnumerable)
				{
					process.Enqueue(item);

					if (finished)
					{
						goto @exitTrue;
					}
				}

				cts.Cancel();

			@exitTrue:

				return await consumeTask;
			}
		}

		public Task ExecuteQuery([NotNull] IWriteQuery<TKey, TValue> writeQuery) => throw new NotImplementedException();
	}
}