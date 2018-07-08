using System.IO;

namespace StringDB.DBTypes.Predefined {
	public class StringType : TypeHandler<string> {
		public override byte Id => 0x02;

		public override long GetLength(string item) => item.GetBytes().Length; //TODO: use length in more modern frameworks
		public override string Read(BinaryReader br)
			=> br.ReadBytes((int)this.ReadLength(br)).GetString();

		public override void Write(BinaryWriter bw, string item) {
			var bytes = item.GetBytes();

			this.WriteLength(bw, bytes.Length);
			bw.Write(bytes);
		}
	}
}