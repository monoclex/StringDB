using StringDB.DBTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {

	internal static class Helpers {

		public static byte[] GetBytes(this string s) => // convert a string to bytes
			Encoding.UTF8.GetBytes(s ?? throw new ArgumentNullException(nameof(s), "A key or value was probably null, can you check?"));

		public static string GetString(this byte[] b) => // convert bytes to a string
			Encoding.UTF8.GetString(b ?? throw new ArgumentNullException(nameof(b), "A key or value was probably null, can you check?"));

		public static IEnumerable<T> AsEnumerable<T>(this T item) { // make a single item enumberable ;D
			yield return item;
		}

		public static void Seek(this Stream s, long p)
			=> s.Seek(p, SeekOrigin.Begin);

		public static T Read<T>(this TypeHandler<T> typeHandler, BinaryReader br)
			=> typeHandler.Read(br, TypeHandlerLengthManager.ReadLength(br));

		//  ___
		// /. .\

		//https://stackoverflow.com/a/33307903

		internal static unsafe bool EqualToFast(this byte[] mainBytes, byte[] compareTo) {
			if (mainBytes == compareTo)
				return true;
			if (mainBytes.Length != compareTo.Length)
				return false;

			fixed (byte* bytes1 = mainBytes, bytes2 = compareTo) {
				var len = mainBytes.Length;
				var rem = len % (sizeof(long) * 16);
				var b1 = (long*)bytes1;
				var b2 = (long*)bytes2;
				var e1 = (long*)(bytes1 + len - rem);

				while (b1 < e1) {
					if (*(b1) != *(b2) || *(b1 + 1) != *(b2 + 1) ||
						*(b1 + 2) != *(b2 + 2) || *(b1 + 3) != *(b2 + 3) ||
						*(b1 + 4) != *(b2 + 4) || *(b1 + 5) != *(b2 + 5) ||
						*(b1 + 6) != *(b2 + 6) || *(b1 + 7) != *(b2 + 7) ||
						*(b1 + 8) != *(b2 + 8) || *(b1 + 9) != *(b2 + 9) ||
						*(b1 + 10) != *(b2 + 10) || *(b1 + 11) != *(b2 + 11) ||
						*(b1 + 12) != *(b2 + 12) || *(b1 + 13) != *(b2 + 13) ||
						*(b1 + 14) != *(b2 + 14) || *(b1 + 15) != *(b2 + 15))
						return false;
					b1 += 16;
					b2 += 16;
				}

				for (var i = 0; i < rem; i++)
					if (mainBytes[len - 1 - i] != compareTo[len - 1 - i])
						return false;

				return true;
			}
		}
	}
}