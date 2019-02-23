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

		[Fact]
		public void CreateWithStreamAndVersion()
		{
			using (var db = StringDatabase.Create(new MemoryStream(), StringDBVersions.v5_0_0, false))
			{
				db.Insert("init", "Hello, World!");
				db.Get("init").Should().Be("Hello, World!");

				db.Should()
					.BeOfType<TransformDatabase<byte[], byte[], string, string>>();
			}
		}
	}
}