﻿using System.IO;

namespace StringDB.DBTypes.Predefined {
	public class StreamType : TypeHandler<Stream> {
		public const int StreamWriterCacheSize = 0x1000;

		public override byte Id => 0x03;

		public override long GetLength(Stream item) => item.Length - item.Position;
		public override void Write(BinaryWriter bw, Stream item) => Write(bw, item, StreamWriterCacheSize);

		public override Stream Read(BinaryReader br) {
			var len = this.ReadLength(br);

			return new StreamFragment(br.BaseStream, br.BaseStream.Position, len);
		}

		public void Write(BinaryWriter bw, Stream item, int cacheSize) {
			bw.Write(GetLength(item));

			var cache = new byte[cacheSize];
			var len = 0;
			while ((len = item.Read(cache, 0, cacheSize)) > 0) {
				bw.Write(cache, 0, len);
			}
		}
	}
}