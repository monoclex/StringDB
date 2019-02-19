using FluentAssertions;
using System.Linq;
using Xunit;

namespace StringDB.Tests
{
	public class DatabaseExtensionTests
	{
		private void EnsureNoValuesLoaded(MockDatabase mdb)
			=> mdb.Data.Where(x => x.Value.Loaded)
			.Should()
			.BeEmpty("No lazy values should be loaded with this operation");

		private void EnsureAllValuesLoaded(MockDatabase mdb)
			=> mdb.Data.Where(kvp => !kvp.Value.Loaded)
			.Should()
			.BeEmpty("All values should be loaded");

		private void EnsureNoValuesLoadedBeyond(MockDatabase mdb, int position)
			=> mdb.Data.Where((kvp, index) => index <= position ? !kvp.Value.Loaded : kvp.Value.Loaded)
			.Should()
			.BeEmpty("No values beyond the current enumeration point should be loaded");

		[Fact]
		public void Keys()
		{
			var mdb = new MockDatabase();

			mdb
				.Keys()
				.Should()
				.BeEquivalentTo
				(
					new []
					{
						"a", "b", "c", "d",
						"a", "b", "c", "d",
						"a", "b"
					},
					"Returns all keys correctly"
				);

			EnsureNoValuesLoaded(mdb);
		}

		[Fact]
		public void Values()
		{
			var mdb = new MockDatabase();

			mdb
				.Values()
				.Cast<MockDatabase.LazyInt>()
				.Select(x => x.Value)
				.Should()
				.BeEquivalentTo
				(
					new []
					{
						0, 1, 2, 3,
						4, 5, 6, 7,
						8, 9
					},
					"Returns all values correctly"
				);

			EnsureNoValuesLoaded(mdb);
		}

		[Fact]
		public void AggresiveValues()
		{
			var mdb = new MockDatabase();

			var enumerator = mdb.ValuesAggresive().GetEnumerator();

			for(var i = 0; i < mdb.Data.Count; i++)
			{
				enumerator.MoveNext().Should().BeTrue();

				EnsureNoValuesLoadedBeyond(mdb, i);
			}

			EnsureAllValuesLoaded(mdb);
		}

		/// <summary>
		/// Eggressively was a typo but i'm keeping it
		/// </summary>
		[Fact]
		public void EnumerateEggressively()
		{
			var mdb = new MockDatabase();

			var enumerator = mdb.EnumerateAggresively(4).GetEnumerator();
			EnsureNoValuesLoaded(mdb);

			for (var i = 1; i <= 3; i++)
			{
				var point = (i * 4) - 1;

				enumerator.MoveNext();
				EnsureNoValuesLoadedBeyond(mdb, point);

				for(var j = 0; j < 4 - 1; j++)
				{
					enumerator.MoveNext();
				}
			}

			EnsureAllValuesLoaded(mdb);
		}
	}
}
