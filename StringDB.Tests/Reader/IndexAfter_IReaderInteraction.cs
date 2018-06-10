using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class IndexAfterTest_IReaderInteraction {
		[Fact]
		public void IndexAfterOneByOne_String() =>
			IndexAfter(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void IndexAfterOneByOne_Bytes() =>
			IndexAfter(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexAfterOneByOne_Stream() =>
			IndexAfter(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IndexAfterOneByTwo_String() =>
			IndexAfter(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void IndexAfterOneByTwo_Bytes() =>
			IndexAfter(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexAfterOneByTwo_Stream() =>
			IndexAfter(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void IndexAfterTwoByOne_String() =>
			IndexAfter(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void IndexAfterTwoByOne_Bytes() =>
			IndexAfter(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexAfterTwoByOne_Stream() =>
			IndexAfter(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IndexAfterTwoByTwo_String() =>
			IndexAfter(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void IndexAfterTwoByTwo_Bytes() =>
			IndexAfter(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexAfterTwoByTwo_Stream() =>
			IndexAfter(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void IndexAfter(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			for (uint i = 0; i < gs.Indexes.Length - 1; i++) {
				var res = sr.IndexAfter(new ReaderInteraction(gs.Indexes[i]));
				Assert.True(res.Index == gs.Indexes[i + 1]);
			}

			Assert.True(sr.IndexAfter(new ReaderInteraction(gs.Indexes[gs.Indexes.Length - 1])) == null);
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}