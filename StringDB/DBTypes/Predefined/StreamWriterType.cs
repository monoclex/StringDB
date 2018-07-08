using System.IO;

namespace StringDB.DBTypes.Predefined {

	internal class StreamType : TypeHandler<Stream> {
		public const int StreamWriterCacheSize = 0x1000;

		public override byte Id => 0x03;

		public override long GetLength(Stream item) => item.Length - item.Position;
		public override void Write(BinaryWriter bw, Stream item) => Write(bw, item, StreamWriterCacheSize);

		public override Stream Read(BinaryReader br, long len)
			=> new StreamFragment(br.BaseStream, br.BaseStream.Position, len);

		public static void Write(BinaryWriter bw, Stream item, int cacheSize) {
			var cache = new byte[cacheSize];
			var len = 0;
			while ((len = item.Read(cache, 0, cacheSize)) > 0) {
				bw.Write(cache, 0, len);
			}
		}
	}
}