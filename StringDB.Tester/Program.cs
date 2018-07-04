using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace StringDB.Tester {

	internal class Program {
		private static void Main(string[] args) {
			using (var db = Database.FromFile("string.db")) {
				foreach (var i in db) // loop over every item and say the index
					Console.WriteLine(i.Index);

				Console.WriteLine(GetSizeOfObject(db));

				var fs = System.IO.File.OpenRead("file-to-insert.txt");

				db.Insert("very-cool", fs);

				db.Insert("test_key", "test_value"); // insert a single item named "test_key"

				db.InsertRange(new KeyValuePair<string, string>[] { // insert multiple items
					new KeyValuePair<string, string>("test1", "value1"),
					new KeyValuePair<string, string>("test2", "value2"),
					new KeyValuePair<string, string>("test3", "value3"),
					new KeyValuePair<string, string>("test4", "value4"),
					new KeyValuePair<string, string>("test5", "value5"),
				});

				foreach (var i in db) // loop over every item in the DB again and say the index
					Console.WriteLine(i.ToString());

				var testKey = db.GetByIndex("test_key"); // get test_key

				Console.WriteLine(testKey.Value); // say the value of test_key

				db.OverwriteValue(testKey, "new_value"); // change the value

				Console.WriteLine(testKey.Value); // say the new value

				db.OverwriteValue(testKey, "newest_value"); // change the value again

				// insert another test_key

				db.Insert("test_key", "i'm hidden behind the other test_key!");

				// foreach loop

				foreach (var i in db)
					Console.WriteLine(i.Index);

				// will print out the first test_key and the other test key

				foreach (var i in db.GetMultipleByIndex("test_key")) //let's get every single test_key
					Console.WriteLine(i.Value); //write out the value

				// now by doing so many tiny inserts we can save a little space if we clean it

				using (var cleaneddb = Database.FromFile("cleaned-string.db")) {
					cleaneddb.CleanFrom(db);
				}
			}

			// let's see hwo big the normal database is compared to the cleaned one

			Console.WriteLine("unclean: " + new System.IO.FileInfo("string.db").Length + " bytes");
			Console.WriteLine("clean: " + new System.IO.FileInfo("cleaned-string.db").Length + " bytes");

			Console.ReadLine();
		}
	}

	public static class GenerateItems {
		public const int ItemsToInsert = 10_000;

		public const int MinIncome = 100;
		public const int MaxIncome = 10_000;

		public const int FriendsToGenerate = 20;

		private static Random _random;
		public static Random Rng => _random ?? (_random = new Random());

		internal static int LastDatabaseIDGenerated = Rng.Next(0, int.MaxValue / 2);

		public static Database NewStringDB() => Database.FromFile(GenerateDatabaseName(LastDatabaseIDGenerated++));

		public static string GenerateDatabaseName(int id) => $"{id}-stringdb.db";

		public static readonly string[] RandomNames = {
			"Jimbo",
			"Josh",
			"Shelby",
			"Kelly",
			"Jimmy",
			"John",
			"Sarah",
			"Hailee",
			"Kevin",
			"Alex",
			"Elizabeth",
			"Skyler"
		};

		public static string RandomName => RandomNames[Rng.Next(0, RandomNames.Length)];

		//we are NOT using yield return because we will get random things *every time* we iterate over it

		public static IEnumerable<Item> GetItems(int items) {
			var res = new List<Item>();

			for (var i = 0; i < items; i++) {
				string usersName = RandomName;
				res.Add(new Item() {
					Identifier = $"{i}.{usersName}",
					Name = $"{usersName} {RandomName}",
					Dollars = Rng.Next(GenerateItems.MinIncome, GenerateItems.MaxIncome),
					Friends = GenerateFriends(GenerateItems.FriendsToGenerate).ToArray()
				});
			}

			return res;
		}

		public static IEnumerable<KeyValuePair<string, string>> GetItemsAsKVP(IEnumerable<Item> items) {
			foreach (var i in items)
				yield return new KeyValuePair<string, string>(i.Identifier, JsonConvert.SerializeObject(i));
		}

		public static IEnumerable<string> GenerateFriends(int amount) {
			for (var i = 0; i < amount; i++)
				yield return RandomName;
		}

		public class Item {
			public string Identifier { get; set; }

			public string Name { get; set; }
			public int Dollars { get; set; }
			public string[] Friends { get; set; }
		}
	}
}