using System.IO;

namespace StringDB.DBTypes.Predefined {

	internal class StringType : TypeHandler<string> {
		public override byte Id => 0x02;

		public override long GetLength(string item) => item.GetBytes().Length; //TODO: use length in more modern frameworks

		public override string Read(BinaryReader br, long len)
			=> br.ReadBytes((int)len).GetString();

		public override void Write(BinaryWriter bw, string item)
			=> bw.Write(item.GetBytes());
	}
}