using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StringDB.Tests {
	public class WriterTests {
		public WriterTests() {
			sampleTest = TestingFileConsts.SingleIndexFile();
			_stream = new MemoryStream();
			db = new Database(_stream, DatabaseMode.Write);
		}

		private Stream _stream { get; set; }
		public SampleTest sampleTest { get; set; }
		public Database db { get; set; }

		private void CompareStreams(Stream a, Stream b) {
			long len = a.Length;

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
			db.Insert("test", "datadatadatadatadatadatadatadata");

			CompareStreams(sampleTest.stream, _stream);

			_stream = new MemoryStream();
			db = new Database(_stream, DatabaseMode.Write);
		}
	}
}