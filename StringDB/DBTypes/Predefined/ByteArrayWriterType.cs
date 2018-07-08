using System.IO;

namespace StringDB.DBTypes.Predefined {

	internal class ByteArrayType : TypeHandler<byte[]> {
		public override byte Id => 0x01;

		public override long GetLength(byte[] item) => item.Length;
		public override byte[] Read(BinaryReader br, long len) => br.ReadBytes((int)len);
		public override void Write(BinaryWriter bw, byte[] item)
			=> bw.Write(item);
	}
}