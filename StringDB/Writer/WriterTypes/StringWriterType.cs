using System.IO;

namespace StringDB.Writer {

	internal class StringWriterType : WriterType<string> {

		public override long Judge(string item) => item.GetBytes().Length;

		public override void Write(BinaryWriter bw, string item) => bw.Write(item.GetBytes());
	}
}