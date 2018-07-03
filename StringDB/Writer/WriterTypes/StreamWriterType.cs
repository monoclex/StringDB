using System.IO;

namespace StringDB.Writer {

	internal class StreamWriterType : WriterType<Stream> {
		public const int StreamWriterCacheSize = 0x1000;

		public override long Judge(Stream item) => item.Length - item.Position;

		public override void Write(BinaryWriter bw, Stream item) => Write(bw, item, StreamWriterCacheSize);

		public void Write(BinaryWriter bw, Stream item, int cacheSize) {
			var cache = new byte[cacheSize];
			var len = 0;
			while ((len = item.Read(cache, 0, cacheSize)) > 0) {
				bw.Write(cache, 0, len);
			}
		}
	}
}