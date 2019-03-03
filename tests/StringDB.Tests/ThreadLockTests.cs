using FluentAssertions;

using StringDB.Databases;
using StringDB.Fluency;

using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests
{
	public class ThreadLockTests
	{
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

			public override void InsertRange(KeyValuePair<int, int>[] items) => Counter++;

			protected override IEnumerable<KeyValuePair<int, ILazyLoader<int>>> Evaluate()
			{
				for (var i = 0; i < 10; i++)
				{
					Counter++;
					yield return new KeyValuePair<int, ILazyLoader<int>>(i, new LazyLoader(this, i));
				}
			}

			public override void Dispose()
			{
			}
		}

		[Fact]
		public async Task MultipleThreadsInsertRange()
		{
			var mockdb = new MockDatabase();

			var db = mockdb.WithThreadLock();

			var tasks = new List<Task>();

			for (var i = 0; i < 10_000; i++)
			{
				tasks.Add(Task.Run(() =>
				{
					for (var j = 0; j < 100; j++)
					{
						db.InsertRange(new KeyValuePair<int, int>[0]);
					}
				}));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);

			// 100 inserts, 10,000 threads
			// 100 * 10,000 = 1,000,000

			mockdb.Counter
				.Should()
				.Be(1_000_000);
		}

		[Fact]
		public async Task MultipleThreadsForeachOver()
		{
			var mockdb = new MockDatabase();

			var db = mockdb.WithThreadLock();

			var tasks = new List<Task>();

			for (var i = 0; i < 10_000; i++)
			{
				tasks.Add(Task.Run(action: () => Parallel.ForEach(db, j => j.Value.Load())));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);

			// 10,000 threads, 10 items each with 1 load
			// 10,000 * (10 * (1 + 1)) = 10,000 * 20 = 200,000

			mockdb.Counter
				.Should()
				.Be(200_000);
		}
	}
}