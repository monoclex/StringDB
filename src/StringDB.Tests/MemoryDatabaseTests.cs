using FluentAssertions;

using System.Linq;

using Xunit;

namespace StringDB.Tests
{
	public class MemoryDatabaseTests
	{
		[Fact]
		public void Works()
		{
			var memdb = new MemoryDatabase<string, string>();

			memdb.Insert("1", "b");
			memdb.Insert("2", "d");
			memdb.Insert("3", "f");
			memdb.Insert("1", "h");

			memdb.Select(x => x.Value.Load()).ToArray()
				.Should()
				.BeEquivalentTo(new string[] { "b", "d", "f", "h" });

			memdb.TryGet("2", out var d)
				.Should().BeTrue();

			d.Should().Be("d");

			memdb.TryGet("1", out var b)
				.Should().BeTrue();

			b.Should().Be("b");

			memdb.GetAll("1").ToArray()
				.Should()
				.BeEquivalentTo(new string[] { "b", "h" });

			var v = memdb.Get("1");
		}
	}
}