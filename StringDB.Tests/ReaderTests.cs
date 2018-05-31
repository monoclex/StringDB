using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StringDB.Tests {
	public class ReaderTests {
		public ReaderTests() {
			sampleTest = TestingFileConsts.SingleIndexFile();
			db = new Database(sampleTest.stream, DatabaseMode.Read);
		}

		public SampleTest sampleTest { get; set; }
		public Database db { get; set; }

		[Fact]
		public void GetsIndexesCorrectly() {
			var indx = db.Indexes();

			for (uint i = 0; i < indx.Length; i++)
				Assert.True(sampleTest.Indexes[i] == indx[i], $"sampleTest.Indexes[{i}] ({sampleTest.Indexes[i]}) != indx[{i}] ({indx[i]})");
		}

		[Fact]
		public void ValuesAreCorrect() {
			var indx = db.Indexes();

			for (uint i = 0; i < indx.Length; i++)
				Assert.True(sampleTest.Datas[i] == db.Get(indx[i]), $"sampleTest.Datas[{i}] ({sampleTest.Datas[i]}) != db.Get(indx[{i}]) ({db.Get(indx[i])})");
		}

		[Fact]
		public void ForeachWorks() {
			var indx = db.Indexes();

			uint c = 0;
			foreach(var i in db) {
				Assert.True(db.Get(indx[c]) == i, $"db.Get(indx[c]) (db.Get(indx[{c}]) (db.Get({indx[c]})) != {i}");

				c++;
			}
		}

		[Fact]
		public void ComplexUsage() {
			foreach(var i in db) {
				for (uint fi = 0; fi < 10; fi++) {
					db.FirstIndex();
					db.Indexes();
					db.Get(sampleTest.Indexes[0]);
				}
			}
		}
	}
}