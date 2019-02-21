using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using StringDB.Databases;
using StringDB.Fluency;
using StringDB.IO;
using StringDB.Transformers;
using Xunit;

namespace StringDB.Tests
{
	public class FluencyTests
	{
		[Fact]
		public void ReturnsMemoryDatabase()
		{
			var db = new DatabaseBuilder()
				.UseMemoryDatabase<int, string>();

			db
				.Should()
				.BeOfType<MemoryDatabase<int, string>>();
		}

		[Fact]
		public void ReturnsIODatabase()
		{
			var db = new DatabaseBuilder()
				.UseIODatabaseProvider(new StoneVaultIODevice(new MemoryStream()));

			db
				.Should()
				.BeOfType<IODatabase>();
		}

		[Fact]
		public void ReturnsTransformDatabase()
		{
			var db = new DatabaseBuilder()
				.UseMemoryDatabase<string, int>()
				.WithTransform(new MockTransformer().Reverse(), new MockTransformer());

			db.Should()
				.BeOfType<TransformDatabase<string, int, int, string>>();
		}
	}
}
