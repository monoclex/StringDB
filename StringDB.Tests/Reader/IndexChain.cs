using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class IndexChainTest {
		[Fact]
		public void IndexChainOneByOne_String() =>
			IndexChain(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void IndexChainOneByOne_Bytes() =>
			IndexChain(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexChainOneByOne_Stream() =>
			IndexChain(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IndexChainOneByTwo_String() =>
			IndexChain(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void IndexChainOneByTwo_Bytes() =>
			IndexChain(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexChainOneByTwo_Stream() =>
			IndexChain(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void IndexChainTwoByOne_String() =>
			IndexChain(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void IndexChainTwoByOne_Bytes() =>
			IndexChain(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexChainTwoByOne_Stream() =>
			IndexChain(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IndexChainTwoByTwo_String() =>
			IndexChain(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void IndexChainTwoByTwo_Bytes() =>
			IndexChain(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexChainTwoByTwo_Stream() =>
			IndexChain(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void IndexChain(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			var c = sr.GetReaderChain();
			Assert.True(c.IndexChain == gs.IndexChain, $"c.IndexChain ({c.IndexChain}) != gs.IndexChain ({gs.IndexChain})");
			Assert.True(c.IndexChainWrite == gs.IndexChainWrite, $"c.IndexChainWrite ({c.IndexChainWrite}) == gs.IndexChainWrite ({gs.IndexChainWrite})");
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}