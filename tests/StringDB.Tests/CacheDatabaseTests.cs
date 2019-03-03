using FluentAssertions;

using StringDB.Databases;
using StringDB.Fluency;

using System.Collections.Generic;

using Xunit;

namespace StringDB.Tests
{
	public class CacheDatabaseTests
	{
		public class MockDatabase : BaseDatabase<string, int>
		{
			public class LazyLoader : ILazyLoader<int>
			{
				private readonly ILazyLoader<int> _inner;

				public LazyLoader(ILazyLoader<int> inner) => _inner = inner;

				public int Loads { get; set; }

				public int Load()
				{
					Loads++;
					return _inner.Load();
				}
			}

			private readonly IDatabase<string, int> _db;
			private readonly List<LazyLoader> _lazyLoaderCache = new List<LazyLoader>();

			public MockDatabase(IDatabase<string, int> db) => _db = db;

			public int InsertRanges { get; set; }

			public override void InsertRange(KeyValuePair<string, int>[] items)
			{
				InsertRanges++;
				_db.InsertRange(items);
			}

			public int Evaluations { get; set; }

			public int Iterations { get; set; }

			protected override IEnumerable<KeyValuePair<string, ILazyLoader<int>>> Evaluate()
			{
				Evaluations++;

				var c = 0;
				foreach (var item in _db)
				{
					if (_lazyLoaderCache.Count <= c)
					{
						_lazyLoaderCache.Add(new LazyLoader(item.Value));
					}

					Iterations++;

					yield return new KeyValuePair<string, ILazyLoader<int>>(item.Key, _lazyLoaderCache[c]);
					c++;
				}
			}

			public override void Dispose() => _db.Dispose();
		}

		[Fact]
		public void CachesProperly()
		{
			var memdb = new DatabaseBuilder()
				.UseMemoryDatabase<string, int>();

			var mock = new MockDatabase(memdb);

			var cache = mock.WithCache();

			memdb.Insert("ichi", 1);
			memdb.Insert("ni", 2);
			memdb.Insert("san", 3);

			for (var i = 0; i < 3; i++)
			{
				foreach (var (key, value) in cache)
				{
					value.Load();
					value.Load();
					value.Load();
				}
			}

			mock.Evaluations
				.Should().Be(3);

			mock.Iterations
				.Should().Be(3 * 3);

			foreach (var (key, value) in mock)
			{
				((MockDatabase.LazyLoader)value)
					.Loads.Should().Be(1);
			}

			cache.Insert("a", 1);
			cache.Insert("a", 2);
			cache.Insert("a", 3);

			mock.InsertRanges
				.Should().Be(3);

			for (var i = 0; i < 3; i++)
			{
				foreach (var item in cache)
				{
					item.Value.Load();
					item.Value.Load();
					item.Value.Load();
				}
			}

			foreach (var (key, value) in mock)
			{
				((MockDatabase.LazyLoader)value)
					.Loads.Should().Be(1);
			}
		}
	}
}