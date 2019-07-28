using StringDB.Querying.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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
		private readonly CancellationToken _cancellationToken;
		private readonly Thread _thread;

		private BufferBlock<KeyValuePair<TKey, ILazyLoader<TValue>>> _bufferBlock;
		private ManualResetEventSlim _mres;

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

			_bufferBlock = new BufferBlock<KeyValuePair<TKey, ILazyLoader<TValue>>>(new DataflowBlockOptions
			{
				BoundedCapacity = 10_000,
				CancellationToken = _cancellationToken,
				EnsureOrdered = false
			});

			_mres = new ManualResetEventSlim();

			_thread = new Thread(() => ThreadCode().GetAwaiter().GetResult());
			_thread.Start();
		}

		private async Task ThreadCode()
		{
			while (!_cancellationToken.IsCancellationRequested)
			{
				_mres.Wait(_cancellationToken);
				_mres.Reset();

				if (_cancellationToken.IsCancellationRequested)
				{
					return;
				}

				int i = 0;
				foreach(var item in _database)
				{
					_bufferBlock.Post(item);

					if (_cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
			}
		}

		public void Dispose()
		{
			if (_disposeDatabase)
			{
				_database.Dispose();
			}
		}

		public async Task<bool> ExecuteQuery(IQuery<TKey, TValue> query)
		{
			ActionBlock<KeyValuePair<TKey, ILazyLoader<TValue>>> consumer = default;

			consumer = new ActionBlock<KeyValuePair<TKey, ILazyLoader<TValue>>>(async result =>
			{
				var acceptance = await query.Process(result.Key, null)
					.ConfigureAwait(false);

				if (acceptance == QueryAcceptance.Completed)
				{
					consumer.Complete();
				}
			});

			_bufferBlock.LinkTo(consumer, new DataflowLinkOptions
			{
			});

			_mres.Set();

			await consumer.Completion;

			return false;
		}

		public async Task ExecuteQuery(IWriteQuery<TKey, TValue> writeQuery)
		{
		}
	}
}