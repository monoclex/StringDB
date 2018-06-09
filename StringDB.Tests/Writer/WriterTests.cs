using StringDB.Writer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Writer {
	//todo: not awful

	public class WriterTests {
		public static List<List<List<KeyValuePair<string, string>>>> Data = new List<List<List<KeyValuePair<string, string>>>>() {
			new List<List<KeyValuePair<string, string>>>() {
				new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("Test1", "Value1")
				}
			},
			new List<List<KeyValuePair<string, string>>>() {
				new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("Test1", "Value1")
				},
				new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("Test2", "Value2")
				}
			},
			new List<List<KeyValuePair<string, string>>>() {
				new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("Test1", "Value1"),
					new KeyValuePair<string, string>("Test2", "Value2")
				}
			},
			new List<List<KeyValuePair<string, string>>>() {
				new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("Test1", "Value1"),
					new KeyValuePair<string, string>("Test2", "Value2")
				},
				new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("Test3", "Value3"),
					new KeyValuePair<string, string>("Test4", "Value4")
				}
			}
		};

		[Fact]
		public void OnePerOne() =>
			TestWriter(
				StreamConsts.OnePerOne(Data[0][0][0].Key, Data[0][0][0].Value),
				Data[0]);

		[Fact]
		public void OnePerTwo() =>
			TestWriter(
				StreamConsts.OnePerTwo(Data[1][0][0].Key, Data[1][0][0].Value, Data[1][1][0].Key, Data[1][1][0].Value),
				Data[1]);

		[Fact]
		public void TwoPerOne() =>
			TestWriter(
				StreamConsts.TwoPerOne(Data[2][0][0].Key, Data[2][0][0].Value, Data[2][0][1].Key, Data[2][0][1].Value),
				Data[2]);

		[Fact]
		public void TwoPerTwo() =>
			TestWriter(
				StreamConsts.TwoPerTwo(Data[3][0][0].Key, Data[3][0][0].Value, Data[3][0][1].Key, Data[3][0][1].Value, Data[3][1][0].Key, Data[3][1][0].Value, Data[3][1][1].Key, Data[3][1][1].Value),
				Data[3]);

		private void TestWriter(GeneratedStream gs, List<List<KeyValuePair<string, string>>> data) {
			var ms = new System.IO.MemoryStream();
			var w = new StreamWriter(ms, DatabaseVersion.Latest, true);

			foreach (var i in data)
				w.InsertRange(i);

			gs.CompareAgainst(ms);
		}
	}
}