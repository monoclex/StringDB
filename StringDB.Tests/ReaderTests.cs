using System.Collections.Generic;
using System.IO;
using Xunit;

namespace StringDB.Tests {

	public class ReaderUnitTests {
		public static readonly Dictionary<string, string> SingleItem = new Dictionary<string, string>() {
				{ "Sample", "Text" }
			};

		public static readonly Dictionary<string, string> OneIndexChain = new Dictionary<string, string>() {
				{ "Item1", "ValueFor1" },
				{ "Item2", "ValueFor2" },
				{ "Item3", "ValueFor3" },
				{ "Item4", "ValueFor4" },
				{ "Item5", "ValueFor5" },
				{ "Item6", "ValueFor6" },
				{ "Item7", "ValueFor7" },
				{ "Item8", "ValueFor8" },
				{ "Item9", "ValueFor9" },
				{ "Item10", "ValueFor10" },
			};

		public static readonly Dictionary<string, string>[] MultipleIndexChains = new Dictionary<string, string>[]
			{ new Dictionary<string, string>() {
				{ "Item1", "ValueFor1" },
				{ "Item2", "ValueFor2" },
				{ "Item3", "ValueFor3" },
			}, new Dictionary<string, string>() {
				{ "Item4", "ValueFor4" },
				{ "Item5", "ValueFor5" },
				{ "Item6", "ValueFor6" },
			}, new Dictionary<string, string>() {
				{ "Item7", "ValueFor7" },
				{ "Item8", "ValueFor8" },
				{ "Item9", "ValueFor9" },
				{ "Item10", "ValueFor10" },
			}
		};

		public static readonly Dictionary<string, string>[] RedundantValues = new Dictionary<string, string>[]
			{ new Dictionary<string, string>() {
				{ "Item1", "ValueFor1_1" },
				{ "Item2", "ValueFor2_1" },
				{ "Item3", "ValueFor3_1" },
			}, new Dictionary<string, string>() {
				{ "Item1", "ValueFor1_2" },
				{ "Item2", "ValueFor2_2" },
				{ "Item3", "ValueFor3_2" },
			}, new Dictionary<string, string>() {
				{ "Item1", "ValueFor1_3" },
				{ "Item2", "ValueFor2_3" },
				{ "Item3", "ValueFor3_3" },
			}
		};

		//TODO: rename these unit tests they're awful

		#region item tests
		[Fact]
		public void SingleItemTest() {
			var rt = new ReaderTest(SingleItem);

			rt.EnsureCanRead();
		}

		[Fact]
		public void TenItems() {
			var rt = new ReaderTest(OneIndexChain);

			rt.EnsureCanRead();
		}

		[Fact]
		public void CanSeekToIndexChain() {
			var rt = new ReaderTest(MultipleIndexChains);

			rt.EnsureCanRead();
		}
		#endregion

		#region Indexes works
		[Fact]
		public void SingleItemIndexesTest() {
			var rt = new ReaderTest(SingleItem);

			rt.IndexesAreCorrect();
		}

		[Fact]
		public void TenIndexesItems() {
			var rt = new ReaderTest(OneIndexChain);

			rt.IndexesAreCorrect();
		}

		[Fact]
		public void CanSeekToIndexIndexesChain() {
			var rt = new ReaderTest(MultipleIndexChains);

			rt.IndexesAreCorrect();
		}
		#endregion

		#region GetValues works
		[Fact]
		public void GetValues() {
			var rt = new ReaderTest(RedundantValues);

			rt.GetValuesWorks();
		}
		#endregion

		#region foreach index
		[Fact]
		public void ForeachLoopIndexInSingleItem() {
			var rt = new ReaderTest(SingleItem);
			
			rt.ForeachLoopIndexCheck();
		}

		[Fact]
		public void ForeachLoopIndexInOneIndexChain() {
			var rt = new ReaderTest(OneIndexChain);
			
			rt.ForeachLoopIndexCheck();
		}

		[Fact]
		public void ForeachLoopIndexInOneMultipleChains() {
			var rt = new ReaderTest(MultipleIndexChains);
			
			rt.ForeachLoopIndexCheck();
		}
		#endregion

		#region foreach value
		[Fact]
		public void ForeachLoopValueInSingleItem() {
			var rt = new ReaderTest(SingleItem);
			
			rt.ForeachLoopValueCheck();
		}

		[Fact]
		public void ForeachLoopValueInOneIndexChain() {
			var rt = new ReaderTest(OneIndexChain);
			
			rt.ForeachLoopValueCheck();
		}

		[Fact]
		public void ForeachLoopValueInOneMultipleChains() {
			var rt = new ReaderTest(MultipleIndexChains);
			
			rt.ForeachLoopValueCheck();
		}
		#endregion

		[Fact]
		public void WriteAndReadWorks() {
			var rt = new ReaderTest(new Dictionary<string, string>() {
				{  "Example", "ExampleValue" }
			});

			rt.EnsureCanRead();
			rt.ForeachLoopIndexCheck();
			rt.ForeachLoopValueCheck();

			rt.AddDict(new Dictionary<string, string>{
				{ "NewTestValue1", "SpecialValue1" },
				{ "NewTestValue2", "SpecialValue2" },
				{ "NewTestValue3", "SpecialValue3" },
			});

			rt.EnsureCanRead();
			rt.ForeachLoopIndexCheck();
			rt.ForeachLoopValueCheck();

			rt.AddDict(new Dictionary<string, string>{
				{ "EvenNewTestValue1", "EvenSpecialValue1" },
				{ "EvenNewTestValue2", "EvenSpecialValue2" },
				{ "EvenNewTestValue3", "EvenSpecialValue3" },
			});

			rt.EnsureCanRead();
			rt.ForeachLoopIndexCheck();
			rt.ForeachLoopValueCheck();
		}
	}

	public class ReaderTest {
		public ReaderTest(params Dictionary<string, string>[] items) {
			this.Items = items;
			this.Db = new Database(new MemoryStream(), DatabaseMode.ReadWrite);

			foreach (var i in items)
				this.Db.InsertRange(i); //the writer unit tests have to pass :)
		}

		public Database Db { get; set; }
		public Dictionary<string, string>[] Items { get; set; }

		public void AddDict(Dictionary<string, string> dict) {
			var vals = this.Items;

			var newSet = new Dictionary<string, string>[vals.Length + 1];

			for (var i = 0; i < vals.Length; i++)
				newSet[i] = vals[i];

			newSet[newSet.Length - 1] = dict;

			this.Items = newSet;

			this.Db.InsertRange(dict);
		}

		public void EnsureCanRead() {
			foreach (var j in this.Items)
				foreach (var i in j) {
					var read = this.Db.GetValueOf(i.Key);

					Assert.True(read == i.Value, $"read ({read}) != i.Value ({i.Value})");
				}
		}

		public void GetValuesWorks() {
			foreach(var i in this.Items[0]) {//todo: not a hacky workaround
				var values = this.Db.GetValuesOf(i.Key);

				Assert.True(values.Length == 3, $"values.Length ({values.Length}) != 3 ({3})");

				for (var j = 0; j < 3; j++)
					Assert.True(values[j] == $"ValueFor{i.Key.Substring(4)}_{j + 1}", $"values[{j}] ({values[j]}) != \"ValueFor{i.Key.Substring(4)}_{j + 1}\"");
			}
		}

		public void IndexesAreCorrect() {
			var indxesL = new List<string>();

			foreach (var j in this.Items)
				foreach (var i in j)
					indxesL.Add(i.Key);

			var indxes = indxesL.ToArray();

			var dbIndxes = this.Db.Indexes();

			Assert.True(indxes.Length == dbIndxes.Length, $"indxes.Length ({indxes.Length}) != dbIndxes.Length ({dbIndxes.Length})");

			for (var i = 0; i < indxes.Length; i++)
				Assert.True(indxes[i] == dbIndxes[i], $"indxes[{i}] ({indxes[i]}) != dbIndxes[{i}] ({dbIndxes[i]})");
		}

		public void ForeachLoopIndexCheck() {
			var len = 0;
			foreach (var i in this.Items)
				len += i.Count;

			var indexes = new string[len];

			var c = 0;
			foreach (var j in this.Items)
				foreach (var i in j) {
					indexes[c] = i.Key;
					c++;
				}

			c = 0;
			foreach (var i in this.Db) {
				Assert.True(indexes[c] == i.Index, $"indexes[{c}] ({indexes[c]}) != i.Index ({i.Index})");
				c++;
			}
		}

		public void ForeachLoopValueCheck() {
			var len = 0;
			foreach (var i in this.Items)
				len += i.Count;

			var indexes = new string[len];

			var c = 0;
			foreach (var j in this.Items)
				foreach (var i in j) {
					indexes[c] = i.Value;
					c++;
				}

			c = 0;
			foreach (var i in this.Db) {
				Assert.True(indexes[c] == i.Value, $"indexes[{c}] ({indexes[c]}) != i.Value ({i.Value})");
				c++;
			}
		}
	}
}