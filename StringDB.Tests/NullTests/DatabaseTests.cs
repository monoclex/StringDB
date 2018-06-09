using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using StringDB;

namespace StringDB.Tests.NullTests {
	public class DatabaseTests {
		[Fact]
		public void FromStream() =>
			Assert.Throws<ArgumentNullException>(() => { Database.FromStream(null); });

		private Database _testDb;
		private Database TestDB =>
			this._testDb ?? (this._testDb = Database.FromStream(StreamConsts.BlankStream()));

		[Fact]
		public void GetValueOf() =>
			Assert.Throws<ArgumentNullException>(() => { this.TestDB.GetValueOf(null); });

		[Fact]
		public void GetValuesOf() =>
			Assert.Throws<ArgumentNullException>(() => { this.TestDB.GetValuesOf(null); });

		[Fact]
		public void StringDBByteOverhead() =>
			Assert.Equal<ulong>(0uL, this.TestDB.StringDBByteOverhead());

		[Fact]
		public void Insert() {
			Assert.Throws<ArgumentNullException>(() => { this.TestDB.Insert(null, null); });
			Assert.Throws<ArgumentNullException>(() => { this.TestDB.Insert(null, "value"); });
			Assert.Throws<ArgumentNullException>(() => { this.TestDB.Insert("index", null); });
		}

		[Fact]
		public void InsertRange() {
			Assert.Throws<ArgumentNullException>(() => { this.TestDB.InsertRange(null); });

			Assert.Throws<ArgumentNullException>(() => {
				this.TestDB.InsertRange(new List<KeyValuePair<string, string>>() {
																					new KeyValuePair<string, string>(null, null)
																				});
			});

			Assert.Throws<ArgumentNullException>(() => {
				this.TestDB.InsertRange(new List<KeyValuePair<string, string>>() {
																					new KeyValuePair<string, string>(null, "value")
																				});
			});

			Assert.Throws<ArgumentNullException>(() => {
				this.TestDB.InsertRange(new List<KeyValuePair<string, string>>() {
																					new KeyValuePair<string, string>("index", null)
																				});
			});
		}

		[Fact]
		public void Indexes() =>
			Assert.Null(this.TestDB.Indexes());

		[Fact]
		public void FirstIndex() =>
			Assert.Null(this.TestDB.FirstIndex());
	}
}