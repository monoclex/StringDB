using FluentAssertions;

using StringDB.Databases;
using StringDB.Transformers;

using System.Collections.Generic;

using Xunit;

namespace StringDB.Tests
{
	public class TransformerDatabaseTests
	{
		[Fact]
		public void InsertRange()
		{
			var db = new MemoryDatabase<string, int>();
			var kt = new MockTransformer();
			var vt = new MockTransformer();

			var tdb = new TransformDatabase<string, int, int, string>
			(
				db: db,
				keyTransformer: new ReverseTransformer<string, int>(kt),
				valueTransformer: vt
			);

			tdb.InsertRange(new KeyValuePair<int, string>[]
			{
				new KeyValuePair<int, string>(1, "a"),
				new KeyValuePair<int, string>(2, "b"),
				new KeyValuePair<int, string>(3, "c"),
			});

			db.EnumerateAggresively(3)
				.Should()
				.BeEquivalentTo(new KeyValuePair<string, int>[]
				{
					new KeyValuePair<string, int>(kt.TransformPre(1), vt.TransformPost("a")),
					new KeyValuePair<string, int>(kt.TransformPre(2), vt.TransformPost("b")),
					new KeyValuePair<string, int>(kt.TransformPre(3), vt.TransformPost("c")),
				});
		}

		[Fact]
		public void Enumerate()
		{
			var db = new MemoryDatabase<string, int>();
			var kt = new MockTransformer();
			var vt = new MockTransformer();

			var tdb = new TransformDatabase<string, int, int, string>
			(
				db: db,
				keyTransformer: new ReverseTransformer<string, int>(kt),
				valueTransformer: vt
			);

			tdb.InsertRange(new KeyValuePair<int, string>[]
			{
				new KeyValuePair<int, string>(1, "a"),
				new KeyValuePair<int, string>(2, "b"),
				new KeyValuePair<int, string>(3, "c"),
			});

			tdb.EnumerateAggresively(3)
				.Should()
				.BeEquivalentTo(new KeyValuePair<int, string>[]
				{
					new KeyValuePair<int, string>(1, "a"),
					new KeyValuePair<int, string>(2, "b"),
					new KeyValuePair<int, string>(3, "c"),
				});
		}
	}
}