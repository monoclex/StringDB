using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StringDB.Tests {

	//once we test the writer to make sure it's good we can then use the writer to generate stuff for the reader to read :D

	public class WriterUnitTests {
		public WriterUnitTests() {

		}

		[Fact]
		public void OneInsertOneIndexChainOverhead() {
			var wt = new WriterTest(null, null, DatabaseMode.ReadWrite);

			wt.InputWriter.Write((byte)4);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 4 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test"));
			wt.InputWriter.Write((byte)Consts.IndexSeperator);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueKey"));

			wt.Db.Insert("Test", "ValueKey");

			var overhead = wt.Db.StringDBByteOverhead();
			var overheadShouldBe = (ulong)(1 + 8 + 1 + 8 + 1 + 1);

			Assert.True(overhead == overheadShouldBe, $"Overheads are not equal, overhead ({overhead}) != overheadShouldBe ({overheadShouldBe})");
		}

		[Fact]
		public void ThreeInsertsOneIndexChainOverhead() {
			var wt = new WriterTest(null, null, DatabaseMode.ReadWrite);

			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + ((8 + 5 + 1) * 3) + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test1"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + ((8 + 5 + 1) * 3) + 4);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test2"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + ((8 + 5 + 1) * 3) + 0);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test3"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf1"));
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf2"));
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf3"));

			wt.Db.InsertRange(new Dictionary<string, string>() {
				{ "Test1", "ValueOf1" },
				{ "Test2", "ValueOf2" },
				{ "Test3", "ValueOf3" },
			});

			var overhead = wt.Db.StringDBByteOverhead();
			var overheadShouldBe = (ulong)(1 + 8 + 1 + 8 + 1 + 8 + 1 + 8 + 1 + 1 + 1 + 1 + 1 + 1);

			Assert.True(overhead == overheadShouldBe, $"Overheads are not equal, overhead ({overhead}) != overheadShouldBe ({overheadShouldBe})");
		}

		[Fact]
		public void TwoInsertsTwoIndexChainsOverhead() {
			var wt = new WriterTest(null, null, DatabaseMode.ReadWrite);

			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test1"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 4 + 6);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf1"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test2"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf2"));

			wt.Db.Insert("Test1", "ValueOf1");
			wt.Db.Insert("Test2", "ValueOf2");

			var overhead = wt.Db.StringDBByteOverhead();
			var overheadShouldBe = (ulong)(1 + 8 + 1 + 8 + 1 + 1 + 1 + 8 + 1 + 8 + 1 + 1);

			Assert.True(overhead == overheadShouldBe, $"Overheads are not equal, overhead ({overhead}) != overheadShouldBe ({overheadShouldBe})");
		}

		[Fact]
		public void OneInsertOneIndexChain() {
			var wt = new WriterTest();

			wt.InputWriter.Write((byte)4);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 4 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test"));
			wt.InputWriter.Write((byte)Consts.IndexSeperator);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueKey"));

			wt.Db.Insert("Test", "ValueKey");

			wt.EnsureEqual();
		}

		[Fact]
		public void ThreeInsertsOneIndexChain() {
			var wt = new WriterTest();

			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + ((8 + 5 + 1) * 3) + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test1"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + ((8 + 5 + 1) * 3) + 4);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test2"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + ((8 + 5 + 1) * 3) + 0);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test3"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf1"));
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf2"));
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf3"));

			wt.Db.InsertRange(new Dictionary<string, string>() {
				{ "Test1", "ValueOf1" },
				{ "Test2", "ValueOf2" },
				{ "Test3", "ValueOf3" },
			});

			wt.EnsureEqual();
		}

		[Fact]
		public void TwoInsertsTwoIndexChains() {
			var wt = new WriterTest();

			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test1"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 4 + 6);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf1"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test2"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf2"));

			wt.Db.Insert("Test1", "ValueOf1");
			wt.Db.Insert("Test2", "ValueOf2");

			wt.EnsureEqual();
		}

		[Fact]
		public void TwoInsertsTwoIndexChains_StopAndPickItBackUp() {
			var wt = new WriterTest();

			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test1"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 4 + 6);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf1"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test2"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf2"));

			wt.Db.Insert("Test1", "ValueOf1");

			wt.Db.Dispose();

			wt.InputWriter.Dispose();
			wt._stream.Dispose();
			wt._output.Dispose();
			wt._stream = new MemoryStream();
			wt._output = new MemoryStream();
			wt.InputWriter = new BinaryWriter(wt._stream);

			wt.Db = new Database(wt._output, DatabaseMode.Write, DatabaseVersion.Latest, true);

			wt.Db.Insert("Test2", "ValueOf2");
			
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test1"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 4 + 6);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf1"));
			wt.InputWriter.Write((byte)5);
			wt.InputWriter.Write((ulong)wt.InputWriter.BaseStream.Position + 8 + 5 + 1 + 8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("Test2"));
			wt.InputWriter.Write((byte)0xFF);
			wt.InputWriter.Write((ulong)0);
			wt.InputWriter.Write((byte)Consts.IsByteValue);
			wt.InputWriter.Write((byte)8);
			wt.InputWriter.Write(Encoding.UTF8.GetBytes("ValueOf2"));

			wt.EnsureEqual();
		}
	}

	public class WriterTest {
		public WriterTest(Stream input = null, Stream output = null, DatabaseMode type = DatabaseMode.Write ) {
			this._stream = input;
			this._output = output;

			if (this._stream == null)
				this._stream = new MemoryStream();

			if (this._output == null)
				this._output = new MemoryStream();

			this.Db = new Database(this._output, type, DatabaseVersion.Latest, true);

			this.InputWriter = new BinaryWriter(this._stream, System.Text.Encoding.UTF8, true);
		}

		public BinaryWriter InputWriter { get; set; }
		public Stream _stream { get; set; }
		public Stream _output { get; set; }
		public Database Db { get; set; }

		public void EnsureEqual() {
			var a = this._stream;
			var b = this._output;

			var len = a.Length;

			if (b.Length < len)
				len = b.Length;

			a.Seek(0, SeekOrigin.Begin);
			b.Seek(0, SeekOrigin.Begin);

			Assert.True(a.Length == b.Length, $"a.Length ({a.Length}) != b.Length ({b.Length}) Lengths are not equal.");

			for (long i = 0; i < len; i++) {
				var byteA = a.ReadByte();
				var byteB = b.ReadByte();
				Assert.True(byteA == byteB, $"At {i}, a.readByte() ({(int)byteA}) != b.readByte() ({(int)byteB})");
			}
		}
	}
}