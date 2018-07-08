using System.IO;

namespace StringDB.DBTypes.Predefined {

	internal class ByteArrayType : TypeHandler<byte[]> {
		public override byte Id => 0x01;

		public override long GetLength(byte[] item) => item.Length;
		public override byte[] Read(BinaryReader br) => br.ReadBytes((int)this.ReadLength(br));
		public override void Write(BinaryWriter bw, byte[] item) {
			this.WriteLength(bw, item.Length);
			bw.Write(item);
		}
	}
}