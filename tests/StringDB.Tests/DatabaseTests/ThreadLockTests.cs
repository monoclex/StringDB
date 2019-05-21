using FluentAssertions;

using StringDB.Databases;
using StringDB.Fluency;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// ensures that multiple threads can write to a thread unsafe database
	/// </summary>
	public class ThreadLockTests
	{
		private readonly MockDatabase _mockdb;
		private readonly IDatabase<int, int> _db;
		private readonly List<Task> _tasks;
		private readonly ManualResetEventSlim _lock;

		public const int IncrementPerLoop = 20;

		public class MockDatabase : BaseDatabase<int, int>
		{
			public class LazyLoader : ILazyLoader<int>
			{
				private readonly MockDatabase _db;
				private readonly int _load;

				public LazyLoader(MockDatabase db, int load)
				{
					_db = db;
					_load = load;
				}

				public int Load()
				{
					_db.Counter++;
					return _load;
				}
			}

			public int Counter;

			public override void InsertRange(params KeyValuePair<int, int>[] items) => Counter++;

			protected override IEnumerable<KeyValuePair<int, ILazyLoader<int>>> Evaluate()
			{
				for (var i = 0; i < IncrementPerLoop / 2; i++)
				{
					Counter++;
					yield return new KeyValuePair<int, ILazyLoader<int>>(i, new LazyLoader(this, i));
				}
			}

			public override void Dispose()
			{
			}
		}

		public ThreadLockTests()
		{
			_mockdb = new MockDatabase();
			_db = _mockdb.WithThreadLock();
			_tasks = new List<Task>();
			_lock = new ManualResetEventSlim();
		}

		/// <summary>
		/// Tests multiple threads inserting into the database.
		/// </summary>
		[Fact]
		public async Task MultipleThreadsInsertRange()
		{
			const int threads = 10_000;
			const int insertRanges = 1_000;

			for (var i = 0; i < threads; i++)
			{
				_tasks.Add(Task.Run(() =>
				{
					_lock.Wait();
					for (var j = 0; j < insertRanges; j++)
					{
						_db.InsertRange(new KeyValuePair<int, int>[0]);
					}
				}));
			}

			_lock.Set();
			await Task.WhenAll(_tasks).ConfigureAwait(false);

			_mockdb.Counter
				.Should()
				.Be(threads * insertRanges);
		}

		/// <summary>
		/// Tests if multiple threads looping over it causes it to fail
		/// </summary>
		[Fact]
		public async Task MultipleThreadsForeachOver()
		{
			const int threads = 10_000;

			for (var i = 0; i < threads; i++)
			{
				_tasks.Add(Task.Run(action: () =>
				{
					_lock.Wait();
					Parallel.ForEach(_db, j => j.Value.Load());
				}));
			}

			_lock.Set();
			await Task.WhenAll(_tasks).ConfigureAwait(false);

			_mockdb.Counter
				.Should()
				.Be(threads * IncrementPerLoop);
		}
	}
}