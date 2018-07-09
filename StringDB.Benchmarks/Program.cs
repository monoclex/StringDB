#define SETUP
#define WRITER_TESTS
#define READER_TESTS
#define CLEAN_TESTS

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Running;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringDB.Benchmarks {

	internal class Program {

		private static void Main() {
			var summary = BenchmarkRunner.Run<StringDBBenchmark>();
			Console.ReadLine();
		}
	}

	[HtmlExporter]
	public class StringDBBenchmark {
#if SETUP
		public Database stringdb;
		private IEnumerable<KeyValuePair<byte[], byte[]>> itemsToInsert;
		private IEnumerable<KeyValuePair<byte[], byte[]>> newInserts;

		private byte[] _begin { get; set; }
		private byte[] _middle { get; set; }
		private byte[] _end { get; set; }

		private int lastdb;

		[GlobalSetup]
		public void SetupMethod() {
			this.itemsToInsert = GenerateItems.GetItemsAsKVP(GenerateItems.GetItems(GenerateItems.ItemsToInsert)).ToList();
			this.newInserts = GenerateItems.GetItemsAsKVP(GenerateItems.GetItems(GenerateItems.ItemsToInsert)).ToList();

			var count = 0;
			foreach (var i in this.itemsToInsert) {
				this._end = i.Key;
				if (count == 0)
					this._begin = i.Key;

				count++;
			}

			var c = 0;
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
			try {
				if (this.stringdb != null)
					this.stringdb.Dispose();
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

#if WRITER_TESTS

		[Benchmark]
		public void InsertRangeItems() => this.stringdb.InsertRange(this.itemsToInsert);

		[Benchmark]
		public void SingleInsertItems() {
			foreach (var i in this.itemsToInsert)
				this.stringdb.Insert(i.Key, i.Value);
		}

		[Benchmark]
		public void OverwriteValues() {
			var enum_1 = this.stringdb.GetEnumerator();
			var enum_2 = this.newInserts.GetEnumerator();

			while (enum_1.MoveNext() && enum_2.MoveNext())
				this.stringdb.OverwriteValue(enum_1.Current, enum_2.Current.Value);
		}

#endif

#if READER_TESTS

		[Benchmark]
		public void IterateThroughEveryEntry() {
			foreach (var i in this.stringdb) { }
		}

		[Benchmark]

		public void IterateThroughEveryEntryAndReadValue() {
			foreach (var i in this.stringdb) {
				var t = i.GetValueAs<string>();
			}
		}

		[Benchmark]
		public void GetValueOfFirst() {
			var t = this.stringdb.Get(this._begin).GetValueAs<string>();
		}

		[Benchmark]
		public void GetValueOfMiddle() {
			var t = this.stringdb.Get(this._middle).GetValueAs<string>();
		}

		[Benchmark]
		public void GetValueOfEnd() {
			var t = this.stringdb.Get(this._end).GetValueAs<string>();
		}

#endif

#if CLEAN_TESTS

		[Benchmark]
		public void CleanFromDatabase() {
			//TODO: remove new StringDb and generate database name from the benchmark
			
			using (var db = GenerateItems.NewStringDB()) {
				db.CleanFrom(this.stringdb);
			}
		}

		[Benchmark]
		public void CleanToDatabase() {
			//TODO: remove new StringDb and generate database name from the benchmark
			
			using (var db = GenerateItems.NewStringDB()) {
				this.stringdb.CleanTo(db);
			}
		}

#endif
#endif
	}

	public static class GenerateItems {
		public const int ItemsToInsert = 100_000;

		public const int MinIncome = 1_000;
		public const int MaxIncome = 10_000;

		public const int FriendsToGenerate = 20;

		private static Random _random;
		public static Random Rng => _random ?? (_random = new Random());

		public static Database NewStringDB() => Database.FromStream(new MemoryStream(), true);

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

		// yield + ToList = works

		public static IEnumerable<Item> GetItems(int items) {
			for (var i = 0; i < items; i++) {
				var usersName = RandomName;
				yield return new Item {
					Identifier = $"{i}.{usersName}",
					Name = $"{usersName} {RandomName}",
					Dollars = Rng.Next(GenerateItems.MinIncome, GenerateItems.MaxIncome),
					Friends = GenerateFriends(GenerateItems.FriendsToGenerate).ToArray()
				};
			}
		}

		public static IEnumerable<KeyValuePair<byte[], byte[]>> GetItemsAsKVP(IEnumerable<Item> items) {
			foreach (var i in items)
				yield return new KeyValuePair<byte[], byte[]>(System.Text.Encoding.UTF8.GetBytes(i.Identifier), System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(i)));
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