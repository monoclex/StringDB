using StringDB.Databases;

using Xunit;

namespace StringDB.Tests
{
	public class TransformerDatabaseTests
	{
		[Fact]
		public void InsertRange()
		{
			var mdb = new MockDatabase();
			var kt = new MockTransformer();
			var vt = new MockTransformer();

			var tdb = new TransformDatabase<string, int, int, string>
			(
				db: mdb,
				keyTransformer: new ReverseTransformer<string, int>(kt),
				valueTransformer: vt
			);
		}

		[Fact]
		public void Enumerate()
		{
			var mdb = new MockDatabase();
			var kt = new MockTransformer();
			var vt = new MockTransformer();

			var tdb = new TransformDatabase<string, int, int, string>
			(
				db: mdb,
				keyTransformer: new ReverseTransformer<string, int>(kt),
				valueTransformer: vt
			);
		}
	}
}