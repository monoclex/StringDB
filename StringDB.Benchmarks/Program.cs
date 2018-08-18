#define SETUP
#define WRITER_TESTS
#define READER_TESTS
#define CLEAN_TESTS

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StringDB.Benchmarks {

	internal class Program {

		private static void Main() {
			var summary = BenchmarkRunner.Run<StringDBBenchmark>();
			Console.ReadLine();
		}
	}

	[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
	[CategoriesColumn]
	[MarkdownExporter]
	[Config(typeof(AllowNonOptimized))]
	public class StringDBBenchmark : IDisposable {

		public class AllowNonOptimized : ManualConfig {
			public AllowNonOptimized() {
				Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS
			}
		}

		public void Dispose() {
			this._db.Dispose();
			this._tmp.Dispose();
			this._db = null;
			this._tmp = null;
			GC.SuppressFinalize(this);
		}

		[Params(GenerateDBType.FilledThreadUnsafe)]
		public GenerateDBType DatabaseType;

		[Params(ItemPosition.Middle)]
		public ItemPosition ItemPosition;

		public IDatabase _db;

		private KeyValuePair<byte[], byte[]> _singleItem;
		private KeyValuePair<byte[], byte[]>[] _itms;

		private Reader.IReaderPair _replRP;
		private byte[] _replIndex;

		public IDatabase _tmp;

		private bool _filled;

		[IterationSetup]
		public void SetupDB() {
			this._itms = GenerateItems.GetItemsAsKVP(GenerateItems.GetItems(GenerateItems.AmountOfItems)).ToArray();
			this._db = GenerateItems.GetDatabase(this.DatabaseType);

			this._filled = this.DatabaseType == GenerateDBType.FilledThreadSafe || this.DatabaseType == GenerateDBType.FilledThreadUnsafe;

			if (this._filled) {
				this._singleItem = GenerateItems.GetItemAsKVP(GenerateItems.GetItems(1).FirstOrDefault());

				switch (this.ItemPosition) {
					case ItemPosition.First: {
						this._replRP = this._db.First();
					}
					break;

					case ItemPosition.Middle: {
						var c = 0;
						foreach (var i in this._db)
							if (c++ == GenerateItems.AmountOfItems / 2) {
								this._replRP = i;
								break;
							}
					}
					break;

					case ItemPosition.End: {
						foreach (var i in this._db)
							this._replRP = i;
					}
					break;

					default: throw new Exception("unknown");
				}

				this._replIndex = this._replRP.Index.GetAs<byte[]>();
			}

			this._tmp = Database.FromStream(new MemoryStream(), true);
		}

		[Benchmark]
		[BenchmarkCategory("Writer")]
		public void Fill() {
			if (this._singleItem.Key == null ||
				this._singleItem.Value == null) throw new Exception("Not intended to be benchmarked");
			this._db.Fill(this._singleItem.Key, this._singleItem.Value, GenerateItems.AmountOfItems);
		}

		[Benchmark]
		[BenchmarkCategory("Writer")]
		public void Insert() {
			if (this._singleItem.Key == null ||
				this._singleItem.Value == null) throw new Exception("Not intended to be benchmarked");
			this._db.Insert(this._singleItem);
		}

		[Benchmark]
		[BenchmarkCategory("Writer")]
		public void InsertRange() {
			if (this._itms == null) throw new Exception("Not intended to be benchmarked");
			this._db.InsertRange(this._itms);
		}

		[Benchmark]
		[BenchmarkCategory("Writer")]
		public void OverwriteValue() {
			if (!this._filled) throw new Exception("Not filled.");

			if (this._replRP == null ||
				this._replIndex == null) throw new Exception("Not intended to be benchmarked");

			this._db.OverwriteValue(this._replRP, this._replIndex);
		}

		[Benchmark]
		[BenchmarkCategory("Reader")]
		public void Get() {
			if (!this._filled) throw new Exception("Not filled.");

			if (this._replIndex == null) throw new Exception("Not intended to be benchmarked");
			this._db.Get(this._replIndex);
		}

		[Benchmark]
		[BenchmarkCategory("Reader")]
		public void TryGet() {
			if (!this._filled) throw new Exception("Not filled.");
			if (this._replIndex == null) throw new Exception("Not intended to be benchmarked");

			this._db.TryGet(this._replIndex, out var _);
		}

		[Benchmark]
		[BenchmarkCategory("Reader")]
		public void GetAll() {
			if (!this._filled) throw new Exception("Not filled.");
			if (this._replIndex == null) throw new Exception("Not intended to be benchmarked");

			this._db.GetAll(this._replIndex);
		}

		[Benchmark]
		[BenchmarkCategory("Database")]
		public void CleanTo() {
			this._db.CleanTo(this._tmp);
		}

		[Benchmark]
		[BenchmarkCategory("Database")]
		public void CleanFrom() {
			this._tmp.CleanFrom(this._db);
		}

		[Benchmark]
		[BenchmarkCategory("Database")]
		public void Flush() {
			this._db.Flush();
		}

		[IterationCleanup]
		public void DisposeDB() {
			this._db.Dispose();
		}
	}

	public enum GenerateDBType {
		BlankThreadUnsafe,
		BlankThreadSafe,
		FilledThreadUnsafe,
		FilledThreadSafe
	}

	public enum ItemPosition {
		First,
		Middle,
		End
	}

	public static class GenerateItems {

		public static IDatabase GetDatabase(GenerateDBType type) {
			var db = Database.FromStream(new MemoryStream(), true);

			switch (type) {
				case GenerateDBType.BlankThreadUnsafe: break;
				case GenerateDBType.BlankThreadSafe: db.MakeThreadSafe(); break;
				case GenerateDBType.FilledThreadUnsafe: db.InsertRange(GenerateItems.GetItemsAsKVP(GenerateItems.GetItems(GenerateItems.AmountOfItems)).ToArray()); break;
				case GenerateDBType.FilledThreadSafe: db.Dispose(); db = GetDatabase(GenerateDBType.FilledThreadUnsafe); db.MakeThreadSafe(); break;
				default: throw new Exception("unknwon");
			}

			return db;
		}

		public const int AmountOfItems = 100_000;

		public const int MinIncome = 1_000;
		public const int MaxIncome = 10_000;

		public const int FriendsToGenerate = 20;

		private static Random _random;
		public static Random Rng => _random ?? (_random = new Random());

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

		// running a yield and a ToList = works

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
				yield return GetItemAsKVP(i);
		}

		public static KeyValuePair<byte[], byte[]> GetItemAsKVP(Item item)
			=> new KeyValuePair<byte[], byte[]>(System.Text.Encoding.UTF8.GetBytes(item.Identifier), System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)));

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