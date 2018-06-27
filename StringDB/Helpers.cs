using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB {
	internal static class Helpers {
		public static byte[] GetBytes(this string s) => Encoding.UTF8.GetBytes(s);
		public static string GetString(this byte[] b) => Encoding.UTF8.GetString(b);
	}
}