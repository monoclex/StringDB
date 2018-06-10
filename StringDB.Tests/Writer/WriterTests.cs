using StringDB.Writer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Writer {
	//todo: not awful

	public class WriterTests {
		[Fact]
		public void OnePerOne_String() =>
			TestWriter(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));

		[Fact]
		public void OnePerOne_Bytes() =>
			TestWriter(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));

		[Fact]
		public void OnePerOne_Stream() =>
			TestWriter(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void OnePerTwo_String() =>
			TestWriter(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));

		[Fact]
		public void OnePerTwo_Bytes() =>
			TestWriter(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));

		[Fact]
		public void OnePerTwo_Stream() =>
			TestWriter(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void TwoPerOne_String() =>
			TestWriter(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));

		[Fact]
		public void TwoPerOne_Bytes() =>
			TestWriter(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));

		[Fact]
		public void TwoPerOne_Stream() =>
			TestWriter(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void TwoPerTwo_String() =>
			TestWriter(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));

		[Fact]
		public void TwoPerTwo_Bytes() =>
			TestWriter(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));

		[Fact]
		public void TwoPerTwo_Stream() =>
			TestWriter(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void TestWriter(GeneratedStream gs) {
			var ms = new System.IO.MemoryStream();
			var w = new StreamWriter(ms, DatabaseVersion.Latest, true);

			if (gs.StreamDat.Data_String() != null)
				foreach (var i in gs.StreamDat.Data_String())
					w.InsertRange(i);
			else if (gs.StreamDat.Data_Bytes() != null)
				foreach (var i in gs.StreamDat.Data_Bytes())
					w.InsertRange(i);
			else if (gs.StreamDat.Data_Stream() != null)
				foreach (var i in gs.StreamDat.Data_Stream())
					w.InsertRange(i);
			else Assert.False(true, $"Unable to find a suitable version of data to insert.");

			gs.CompareAgainst(ms);
		}
	}
}