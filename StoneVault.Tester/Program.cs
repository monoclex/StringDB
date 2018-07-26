using StringDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StoneVault.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
			using (var vault = new MemoryStream())
			using (var db = Database.FromStream(new MemoryStream(), true)) {
				db.Fill("EXAMPLE KEY", " ~ v a l u e ~ ", 25);

				Vault.Store(ReadOut(db), vault);

				vault.Seek(0, SeekOrigin.Begin);

				int items = 0;

				foreach(var i in Vault.Read(vault)) {
					Console.WriteLine($"[{Encoding.UTF8.GetString(i.Key)}, {Encoding.UTF8.GetString(i.Value)}]");
					items++;
				}

				Console.WriteLine($"Items: {items}");
			}

			Console.ReadLine();
        }

		static IEnumerable<KeyValuePair<byte[], byte[]>> ReadOut(IDatabase db) {
			foreach (var i in db)
				yield return new KeyValuePair<byte[], byte[]>(i.Index.GetAs<byte[]>(), i.Value.GetAs<byte[]>());
		}
    }
}
