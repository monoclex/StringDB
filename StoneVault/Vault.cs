using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StoneVault {

	/// <summary>Vault - Store your data in a stone, unchangeable format for transferring to and from new StringDB versions.</summary>
	public static class Vault {

		private const byte DATA_GOOD = 0x00;
		private const byte DATA_END = 0xFF;
		
		/// <param name="encoding">Default ( null ) is UTF8</param>
		public static void Store(IEnumerable<KeyValuePair<byte[], byte[]>> vaultItems, Stream writeTo, Encoding encoding = null) {
			using (var bw = new BinaryWriter(writeTo, encoding ?? Encoding.UTF8, true)) {
				foreach (var i in vaultItems) {
					WriteByteArray(bw, i.Key); // write the key and value
					WriteByteArray(bw, i.Value);
				}
				bw.Write(DATA_END); // data has ended

				bw.Flush(); // flush
			}
		}

		/// <param name="encoding">Default ( null ) is UTF8</param>
		public static IEnumerable<KeyValuePair<byte[], byte[]>> Read(Stream readFrom, Encoding encoding = null) {
			bool toggle = false;
			byte[] last = new byte[0];
			foreach(var i in ReadOut(readFrom, encoding)) {
				if(toggle)
					yield return new KeyValuePair<byte[], byte[]>(last, i);
				else last = i;

				toggle = !toggle;
			}
		}

		public static void WriteIn(IEnumerable<byte[]> vaultItems, Stream writeTo, Encoding encoding = null) {
			using (var bw = new BinaryWriter(writeTo, encoding ?? Encoding.UTF8, true)) {
				foreach (var i in vaultItems)
					WriteByteArray(bw, i);
				bw.Write(DATA_END);

				bw.Flush();
			}
		}

		/// <summary>StoneVault's format is based on individual byte arrays, which is just meshed together for Store & Write</summary>
		public static IEnumerable<byte[]> ReadOut(Stream readFrom, Encoding encoding = null) {
			using (var br = new BinaryReader(readFrom, encoding ?? Encoding.UTF8, true))
				while (br.ReadByte() != DATA_END) {
					var len = br.ReadInt64();

					if (len > int.MaxValue) throw new ArgumentOutOfRangeException($"Unable to read out as a byte array - the length is over the maximum int value {int.MaxValue}");
					yield return br.ReadBytes((int)len);
				}
		}

		private static void WriteByteArray(BinaryWriter bw, byte[] item) {
			bw.Write(DATA_GOOD); // the byte array is good for reading signal

			bw.Write((long)item.Length); // write the length of it ( as a long, for future Stream implementations )
			bw.Write(item); // write the data
		}
	}
}
