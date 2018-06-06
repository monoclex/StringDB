using System.IO;

namespace StringDB.Tests {
	public static class TestingFileConsts {
		public const string keyName = "test";
		public const string keyValue = "datadatadatadatadatadatadatadata";

		public static SampleTest SingleIndexFile() {
			var ms = new MemoryStream();
			var bw = new BinaryWriter(ms);

			bw.Write((byte)keyName.Length); //length of key 1 byte (1)
			bw.Write((ulong)(1 + 8 + keyName.Length + 1 + 8)); //position where the key will be 8 bytes (9)
			foreach (var i in keyName) bw.Write(i); //key name 4 bytes (13)
			bw.Write((byte)0xFF); //index seperator1 byte (14)
			bw.Write((ulong)0); //no new index chain 8 bytes (22)
			bw.Write((int)keyValue.Length); //length of data 4 bytes (26)
			foreach (var i in keyValue) bw.Write(i); //the data 32 bytes (58)

			return new SampleTest(new string[] { keyName }, new string[] { keyValue }, ms);
		}

		public static SampleTest ThreeIndexesTheSameFile() {
			var ms = new MemoryStream();
			var bw = new BinaryWriter(ms);

			var calc = (1 + 8 + keyName.Length + 1 + 8 + 4 + keyValue.Length);

			for (uint j = 0; j < 3; j++) {
				var totalPos = 0L;

				if(j > 0)
					totalPos = calc * j;

				bw.Write((byte)keyName.Length); //length of key 1 byte (1)
				bw.Write((ulong)(1 + 8 + keyName.Length + 1 + 8 + totalPos)); //position where the key will be 8 bytes (9)
				foreach (var i in keyName) bw.Write(i); //key name 4 bytes (13)
				bw.Write((byte)0xFF); //index seperator1 byte (14)
				bw.Write((ulong)(totalPos + calc + 1 > calc * 3 ? 0 : totalPos + calc)); //no new index chain 8 bytes (22)
				bw.Write((int)keyValue.Length); //length of data 4 bytes (26)
				foreach (var i in keyValue) bw.Write(i); //the data 32 bytes (58)
			}

			return new SampleTest(new string[] { keyName, keyName, keyName }, new string[] { keyValue, keyValue, keyValue }, ms);
		}
	}

	public class SampleTest {
		public SampleTest(string[] indexes, string[] datas, Stream stream) {
			this.Indexes = indexes;
			this.Datas = datas;
			this._stream = stream;
		}

		public Stream _stream { get; set; }
		public string[] Indexes { get; set; }
		public string[] Datas { get; set; }
	}
}