using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class OverheadTest {
		[Fact]
		public void OverheadOneByOne_String() =>
			OverheadOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void OverheadOneByOne_Bytes() =>
			OverheadOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void OverheadOneByOne_Stream() =>
			OverheadOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void OverheadOneByTwo_String() =>
			OverheadOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void OverheadOneByTwo_Bytes() =>
			OverheadOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void OverheadOneByTwo_Stream() =>
			OverheadOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void OverheadTwoByOne_String() =>
			OverheadOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void OverheadTwoByOne_Bytes() =>
			OverheadOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void OverheadTwoByOne_Stream() =>
			OverheadOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void OverheadTwoByTwo_String() =>
			OverheadOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void OverheadTwoByTwo_Bytes() =>
			OverheadOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void OverheadTwoByTwo_Stream() =>
			OverheadOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void OverheadOf(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			Assert.True(sr.GetOverhead() == gs.Overhead, $"Overheads aren't equal: sr.GetOverhead() ({sr.GetOverhead()}) != gs.Overhead ({gs.Overhead})");
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}