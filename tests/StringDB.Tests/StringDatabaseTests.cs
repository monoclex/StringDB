using FluentAssertions;

using StringDB.Databases;
using StringDB.IO;

using System.IO;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests for the <see cref="StringDatabase"/> helper class.
	/// </summary>
	public class StringDatabaseTests
	{
		private void Insert(IDatabase<string, string> db)
		{
			db.Insert("init", "Hello, World!");
			db.Get("init").Should().Be("Hello, World!");
		}

		/// <summary>
		/// Upon using the Create method, a MemoryDatabase should be constructed.
		/// </summary>
		[Fact]
		public void Create()
		{
			using (var db = StringDatabase.Create())
			{
				Insert(db);

				db.Should()
					.BeOfType<MemoryDatabase<string, string>>();
			}
		}

		/// <summary>
		/// Assures that upon creating a database with a backing MemoryStream,
		/// it uses a TransformDatabase.
		/// </summary>
		[Fact]
		public void CreateWithStream()
		{
			using (var db = StringDatabase.Create(new MemoryStream()))
			{
				Insert(db);

				db.Should()
					.BeOfType<TransformDatabase<byte[], byte[], string, string>>();
			}
		}

		/// <summary>
		/// Assures that upon creating a database with a backing MemoryStream,
		/// it uses the proper StringDB version and a memory stream to back it.
		/// </summary>
		/// <param name="version">The version to use.</param>
		[Theory]
		[InlineData(StringDBVersions.v5_0_0)]
		[InlineData(StringDBVersions.v10_0_0)]
		public void CreateWithStreamAndVersion(StringDBVersions version)
		{
			using (var db = StringDatabase.Create(new MemoryStream(), version, false))
			{
				Insert(db);

				db.Should()
					.BeOfType<TransformDatabase<byte[], byte[], string, string>>();
			}
		}
	}
}