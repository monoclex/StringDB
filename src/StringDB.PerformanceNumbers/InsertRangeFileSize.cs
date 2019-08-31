using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StringDB.PerformanceNumbers
{
	public class InsertRangeFileSize
	{
		public void Run()
		{
			void Insert(IDatabase<string, string> db, int c)
			{
				var kvp = GenerateKeyValuePair(128, 1024);

				db.InsertRange(Enumerable.Repeat(kvp, c).ToArray());
			}

			var size1 = GetSizeAfter(1, Insert);
			var size2 = GetSizeAfter(50, Insert);
			var size3 = GetSizeAfter(100, Insert);

			Console.WriteLine($"Size after 1 elements in an insert range: {size1}");
			Console.WriteLine($"Size after 50 elements in an insert range: {size2}");
			Console.WriteLine($"Size after 100 elements in an insert range: {size3}");
		}

		public static long GetSizeAfter(int kvps, Action<IDatabase<string, string>, int> action)
		{
			using (var ms = new MemoryStream())
			using (var db = StringDatabase.Create(ms))
			{
				action(db, kvps);

				return ms.Length;
			}
		}

		public static KeyValuePair<string, string> GenerateKeyValuePair(int keySize, int valueSize)
		{
			var key = new string('X', keySize);
			var value = new string('X', valueSize);

			return new KeyValuePair<string, string>(key, value);
		}
	}
}