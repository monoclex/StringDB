using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace StringDB.Tests
{
	public class BaseDatabaseTests
	{
		[Fact]
		public void Insert()
		{
			var mbdb = new MockBaseDatabase();

			mbdb.Insert("str", 5);

			mbdb.Inserted
					.Should()
					.HaveCount(1, "Only 1 item should be inserted")
				.And
					.BeEquivalentTo
					(
						new KeyValuePair<string, int>[]
						{
							new KeyValuePair<string, int>("str", 5)
						},
						"An array with 1 item should be inserted upon single insert"
					);
		}

		[Fact]
		public void GetAll()
		{
			var mbdb = new MockBaseDatabase();

			mbdb.EnsureNoValuesLoaded();

			foreach (var item in mbdb.GetAll("a"))
			{
				mbdb.EnsureNoValuesLoaded();
			}

			mbdb.EnsureNoValuesLoaded();
		}
	}
}