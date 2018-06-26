using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StringDB.Benchmarks {
	class Program {
		static void Main(string[] args) {
			var summary = BenchmarkRunner.Run<StringDBBenchmark>();
			try {
				System.IO.File.WriteAllText("results.txt", JsonConvert.SerializeObject(summary));
			} catch { }
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
			"Amber",
			"Skyler"
		};

		public static string RandomName => RandomNames[Rng.Next(0, RandomNames.Length)];

		//we are NOT using yield return because we will get random things *every time* we iterate over it

		public static IEnumerable<Item> GetItems(int items) {
			var res = new List<Item>(items);

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

	[HtmlExporter]
	public class StringDBBenchmark {
		public Database stringdb;
		private IEnumerable<KeyValuePair<string, string>> itemsToInsert;
		private IEnumerable<KeyValuePair<string, string>> newInserts;

		private string _begin { get; set; }
		private string _middle { get; set; }
		private string _end { get; set; }

		private int lastdb = 0;

		[GlobalSetup]
		public void SetupMethod() {
			this.itemsToInsert = GenerateItems.GetItemsAsKVP(GenerateItems.GetItems(GenerateItems.ItemsToInsert));
			this.newInserts = GenerateItems.GetItemsAsKVP(GenerateItems.GetItems(GenerateItems.ItemsToInsert));

			int count = 0;
			foreach(var i in this.itemsToInsert) {
				this._end = i.Key;
				if (count == 0)
					this._begin = i.Key;
			}

			int c = 0;
			foreach (var i in this.itemsToInsert) {
				if (count / 2 == c)
					this._middle = i.Key;
				c++;
			}
		}

		[IterationSetup]
		public void IterationSetup() {
			this.stringdb = GenerateItems.NewStringDB();
			this.stringdb.InsertRange(this.itemsToInsert);
		}

		[IterationCleanup]
		public void IterationCleanup() {
			this.lastdb = GenerateItems.LastDatabaseIDGenerated;

			try {

				if (this.stringdb != null)
					this.stringdb.Dispose();

				File.Delete(GenerateItems.GenerateDatabaseName(this.lastdb));

			} catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
		
		[Benchmark]
		public void InsertRangeItems() {
			this.stringdb.InsertRange(this.itemsToInsert);
		}

		[Benchmark]
		public void SingleInsertItems() {
			foreach(var i in this.itemsToInsert)
				this.stringdb.Insert(i.Key, i.Value);
		}
		
		[Benchmark]
		public void OverwriteValues() {
			var enum_1 = this.stringdb.GetEnumerator();
			var enum_2 = this.newInserts.GetEnumerator();

			while (enum_1.MoveNext() && enum_2.MoveNext())
				this.stringdb.OverwriteValue(enum_1.Current, enum_2.Current.Value);
		}
		
		[Benchmark]
		public void IterateThroughEveryEntry() {
			foreach (var i in this.stringdb) { }
		}

		[Benchmark]
		public void GetValueOfFirst() {
			var t = this.stringdb.GetByIndex(this._begin).Value;
		}

		[Benchmark]
		public void GetValueOfMiddle() {
			var t = this.stringdb.GetByIndex(this._middle).Value;
		}

		[Benchmark]
		public void GetValueOfEnd() {
			var t = this.stringdb.GetByIndex(this._end).Value;
		}

		[Benchmark]
		public void IterateThroughEveryEntryAndReadValue() {
			foreach (var i in this.stringdb) {
				var t = i.Value;
			}
		}

		[Benchmark]
		public void CleanFromDatabase() {
			//TODO: remove new StringDb and generate database name from the benchmark

			var clean = GenerateItems.LastDatabaseIDGenerated;
			using (var db = GenerateItems.NewStringDB()) {
				db.CleanFrom(this.stringdb);
			}
			File.Delete(GenerateItems.GenerateDatabaseName(clean));
		}

		[Benchmark]
		public void CleanToDatabase() {
			//TODO: remove new StringDb and generate database name from the benchmark

			var clean = GenerateItems.LastDatabaseIDGenerated;
			using (var db = GenerateItems.NewStringDB()) {
				this.stringdb.CleanTo(db);
			}

			File.Delete(GenerateItems.GenerateDatabaseName(clean));
		}
	}
}
