using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace StringDB.PerformanceNumbers
{
	public class SingleInsertFileSize
	{
		public void Run()
		{
			Action<IDatabase<string, string>> insert = (db) => DoWrite(db, 128, 1024);

			var size1 = GetSizeAfter(1, insert);
			var size2 = GetSizeAfter(50, insert);
			var size3 = GetSizeAfter(100, insert);

			Console.WriteLine($"Size after 1 single insert: {size1}");
			Console.WriteLine($"Size after 50 single inserts: {size2}");
			Console.WriteLine($"Size after 100 single inserts: {size3}");

			Console.ReadLine();
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
			string key = new string('X', keySize);
			string value = new string('X', valueSize);

			db.Insert(key, value);
		}
	}
}
