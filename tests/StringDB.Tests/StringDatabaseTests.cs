using FluentAssertions;

using StringDB.Databases;
using StringDB.IO;

using System.IO;

using Xunit;

namespace StringDB.Tests
{
	public class StringDatabaseTests
	{
		[Fact]
		public void Create()
		{
			using (var db = StringDatabase.Create())
			{
				db.Insert("init", "Hello, World!");
				db.Get("init").Should().Be("Hello, World!");

				db.Should()
					.BeOfType<MemoryDatabase<string, string>>();
			}
		}

		[Fact]
		public void CreateWithStream()
		{
			using (var db = StringDatabase.Create(new MemoryStream()))
			{
				db.Insert("init", "Hello, World!");
				db.Get("init").Should().Be("Hello, World!");

				db.Should()
					.BeOfType<TransformDatabase<byte[], byte[], string, string>>();
			}
		}

		[Theory]
		[InlineData(StringDBVersions.v5_0_0)]
		[InlineData(StringDBVersions.v10_0_0)]
		public void CreateWithStreamAndVersion(StringDBVersions version)
		{
			using (var db = StringDatabase.Create(new MemoryStream(), version, false))
			{
				db.Insert("init", "Hello, World!");
				db.Get("init").Should().Be("Hello, World!");

				db.Should()
					.BeOfType<TransformDatabase<byte[], byte[], string, string>>();
			}
		}
	}
}