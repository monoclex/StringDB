using FluentAssertions;

using System;
using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace StringDB.Tests
{
	public class BaseDatabaseTests
	{
		[Fact]
		public void Get()
		{
			var mbdb = new MockBaseDatabase();

			mbdb.Get("a")
				.Should()
				.Be(0);

			mbdb.Get("d")
				.Should()
				.Be(3);

			Action throws = () => mbdb.Get("e");

			throws
				.Should()
				.ThrowExactly<KeyNotFoundException>();
		}

		[Fact]
		public void TryGet()
		{
			var mbdb = new MockBaseDatabase();

			mbdb.TryGet("a", out var result)
				.Should()
				.BeTrue();

			result
				.Should()
				.Be(0);

			mbdb.TryGet("d", out result)
				.Should()
				.BeTrue();

			result
				.Should()
				.Be(3);

			mbdb.TryGet("e", out result)
				.Should()
				.BeFalse();

			result
				.Should()
				.Be(default);
		}

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

		[Fact]
		public void GetEnumerator()
		{
			var mbdb = new MockBaseDatabase();

			mbdb.GetEnumerator()
				.Should()
				.BeEquivalentTo(mbdb.Enumerator());

			((IEnumerator)mbdb.GetEnumerator())
				.Should()
				.BeEquivalentTo(mbdb.Enumerator());
		}
	}
}