using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StringDB.Tests {
	public class WriterTests {
		public WriterTests() {
			this._sampleTest = TestingFileConsts.SingleIndexFile();
			this._stream = new MemoryStream();
			this._db = new Database(this._stream, DatabaseMode.Write);
		}

		private Stream _stream { get; set; }
		public SampleTest _sampleTest { get; set; }
		public Database _db { get; set; }

		private void CompareStreams(Stream a, Stream b) {
			var len = a.Length;

			if (b.Length < len)
				len = b.Length;

			a.Seek(0, SeekOrigin.Begin);
			b.Seek(0, SeekOrigin.Begin);

			for (long i = 0; i < len; i++)
				Assert.True(a.ReadByte() == b.ReadByte(), $"At {i}, a.readByte() != b.readByte()");

			Assert.True(a.Length == b.Length, "Lengths are not equal.");
		}

		[Fact]
		public void SingleInsert() {
			this._db.Insert(TestingFileConsts.keyName, TestingFileConsts.keyValue);

			CompareStreams(this._sampleTest._stream, this._stream);

			this._stream = new MemoryStream();
			this._db = new Database(this._stream, DatabaseMode.Write);
		}
	}
}