using FluentAssertions;

using System.Linq;

using Xunit;

namespace StringDB.Tests
{
	public class DatabaseExtensionTests
	{
		[Fact]
		public void Keys()
		{
			var mdb = new MockDatabase();

			mdb
				.Keys()
				.Should()
				.BeEquivalentTo
				(
					new[]
					{
						"a", "b", "c", "d",
						"a", "b", "c", "d",
						"a", "b"
					},
					"Returns all keys correctly"
				);

			mdb.EnsureNoValuesLoaded();
		}

		[Fact]
		public void Values()
		{
			var mdb = new MockDatabase();

			mdb
				.Values()
				.Cast<LazyItem<int>>()
				.Select(x => x.Value)
				.Should()
				.BeEquivalentTo
				(
					new[]
					{
						0, 1, 2, 3,
						4, 5, 6, 7,
						8, 9
					},
					"Returns all values correctly"
				);

			mdb.EnsureNoValuesLoaded();
		}

		[Fact]
		public void AggresiveValues()
		{
			var mdb = new MockDatabase();

			var enumerator = mdb.ValuesAggressive().GetEnumerator();

			for (var i = 0; i < mdb.Data.Count; i++)
			{
				enumerator.MoveNext().Should().BeTrue();

				mdb.EnsureNoValuesLoadedBeyond(i);
			}

			mdb.EnsureAllValuesLoaded();
		}

		/// <summary>
		/// Eggressively was a typo but i'm keeping it
		/// </summary>
		[Fact]
		public void EnumerateEggressively()
		{
			var mdb = new MockDatabase();

			var enumerator = mdb.EnumerateAggressively(4).GetEnumerator();
			mdb.EnsureNoValuesLoaded();

			for (var i = 1; i <= 3; i++)
			{
				var point = (i * 4) - 1;

				enumerator.MoveNext();
				mdb.EnsureNoValuesLoadedBeyond(point);

				for (var j = 0; j < 4 - 1; j++)
				{
					enumerator.MoveNext();
				}
			}

			mdb.EnsureAllValuesLoaded();
		}
	}
}