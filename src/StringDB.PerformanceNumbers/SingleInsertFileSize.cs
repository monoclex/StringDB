using System;
using System.IO;

namespace StringDB.PerformanceNumbers
{
	public class SingleInsertFileSize
	{
		public void Run()
		{
			void Insert(IDatabase<string, string> db) => DoWrite(db, 128, 1024);

			var size1 = GetSizeAfter(1, Insert);
			var size2 = GetSizeAfter(50, Insert);
			var size3 = GetSizeAfter(100, Insert);

			Console.WriteLine($"Size after 1 single insert: {size1}");
			Console.WriteLine($"Size after 50 single inserts: {size2}");
			Console.WriteLine($"Size after 100 single inserts: {size3}");
		}

		public static long GetSizeAfter(int times, Action<IDatabase<string, string>> action)
		{
			using (var ms = new MemoryStream())
			using (var db = StringDatabase.Create(ms))
			{
				for (var i = 0; i < times; i++)
				{
					action(db);
				}

				return ms.Length;
			}
		}

		public static void DoWrite(IDatabase<string, string> db, int keySize, int valueSize)
		{
			var key = new string('X', keySize);
			var value = new string('X', valueSize);

			db.Insert(key, value);
		}
	}
}