using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StringDB.Tests {
	public class ReaderTests {
		public ReaderTests() {
			this._sampleTest = TestingFileConsts.SingleIndexFile();
			this._complexsampleTest = TestingFileConsts.ThreeIndexesTheSameFile();
			this._db = new Database(this._sampleTest._stream, DatabaseMode.ReadWrite);
			this._complexdb = new Database(this._complexsampleTest._stream, DatabaseMode.ReadWrite);
		}

		public SampleTest _sampleTest { get; set; }
		public SampleTest _complexsampleTest { get; set; }
		public Database _db { get; set; }
		public Database _complexdb { get; set; }

		[Fact]
		public void GetsIndexesCorrectly() {
			var indx = this._db.Indexes();

			for (uint i = 0; i < indx.Length; i++)
				Assert.True(this._sampleTest.Indexes[i] == indx[i], $"sampleTest.Indexes[{i}] ({this._sampleTest.Indexes[i]}) != indx[{i}] ({indx[i]})");
		}

		[Fact]
		public void ValuesAreCorrect() {
			var indx = this._db.Indexes();

			for (uint i = 0; i < indx.Length; i++)
				Assert.True(this._sampleTest.Datas[i] == this._db.GetValueOf(indx[i]), $"sampleTest.Datas[{i}] ({this._sampleTest.Datas[i]}) != db.Get(indx[{i}]) ({this._db.GetValueOf(indx[i])})");
		}

		[Fact]
		public void ForeachWorks() {
			var indx = this._db.Indexes();

			uint c = 0;
			foreach(var i in this._db) {
				Assert.True(this._db.GetValueOf(indx[c]) == i.Value, $"db.Get(indx[c]) (db.Get(indx[{c}]) (db.Get({indx[c]})) != {i}");

				c++;
			}
		}

		[Fact]
		public void ComplexUsage() {
			foreach (var i in this._db) {
				for (uint fi = 0; fi < 10; fi++) {
					this._db.FirstIndex();
					this._db.Indexes();
					this._db.GetValueOf(this._sampleTest.Indexes[0]);
				}
			}
		}

		[Fact]
		public void MultipleIndexesAreFine() {
			var vals = this._complexdb.GetValuesOf(this._complexsampleTest.Indexes[0]);

			Assert.True(vals.Length == this._complexsampleTest.Indexes.Length, $"vals.Length ({vals.Length}) != complexsampleTest.Indexes.Length ({this._complexsampleTest.Indexes.Length})");

			for (var i = 0; i < vals.Length; i++)
				Assert.True(vals[i] == this._complexsampleTest.Datas[i], $"vals[{i}] ({vals[i]}) != complexsampleTest.Datas[{i}] ({this._complexsampleTest.Datas[i]})");
		}
	}
}