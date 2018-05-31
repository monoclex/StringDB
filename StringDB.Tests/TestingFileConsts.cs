using System;
using System.IO;
using Xunit;

namespace StringDB.Tests {
	public static class TestingFileConsts {
		public static SampleTest SingleIndexFile() {
			var ms = new MemoryStream();
			var bw = new BinaryWriter(ms);

			bw.Write((byte)4); //1 byte (1)
			bw.Write((ulong)22); //8 bytes (9)
			foreach (var i in "test") bw.Write(i); //4 bytes (13)
			bw.Write((byte)0xFF); //1 byte (14)
			bw.Write((ulong)0); //8 bytes (22)
			bw.Write((int)32); //4 bytes (26)
			foreach (var i in "datadatadatadatadatadatadatadata") bw.Write(i); //32 bytes (58)

			return new SampleTest(new string[] { "test" }, new string[] { "datadatadatadatadatadatadatadata" }, ms);
		}
	}

	public class SampleTest {
		public SampleTest(string[] indexes, string[] datas, Stream _stream) {
			this.Indexes = indexes;
			this.Datas = datas;
			this.stream = _stream;
		}

		public Stream stream { get; set; }
		public string[] Indexes { get; set; }
		public string[] Datas { get; set; }
	}
}