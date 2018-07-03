using System.IO;

namespace StringDB.Writer {

	internal class ByteArrayWriterType : WriterType<byte[]> {

		public override long Judge(byte[] item) => item.Length;

		public override void Write(BinaryWriter bw, byte[] item) => bw.Write(item);
	}
}