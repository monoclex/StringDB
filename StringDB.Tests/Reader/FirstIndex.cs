using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class FirstIndexTest {
		[Fact]
		public void FirstIndexOneByOne_String() =>
			FirstIndex(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void FirstIndexOneByOne_Bytes() =>
			FirstIndex(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void FirstIndexOneByOne_Stream() =>
			FirstIndex(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void FirstIndexOneByTwo_String() =>
			FirstIndex(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void FirstIndexOneByTwo_Bytes() =>
			FirstIndex(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void FirstIndexOneByTwo_Stream() =>
			FirstIndex(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void FirstIndexTwoByOne_String() =>
			FirstIndex(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void FirstIndexTwoByOne_Bytes() =>
			FirstIndex(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void FirstIndexTwoByOne_Stream() =>
			FirstIndex(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void FirstIndexTwoByTwo_String() =>
			FirstIndex(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void FirstIndexTwoByTwo_Bytes() =>
			FirstIndex(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void FirstIndexTwoByTwo_Stream() =>
			FirstIndex(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void FirstIndex(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			Assert.True(sr.FirstIndex().Index == gs.Indexes[0], $"sr.FirstIndex().Index != gs.Indexes[0]");
			Assert.True(sr.FirstIndex().IndexChainPassedAmount == 0, $"sr.FirstIndex().IndexChainPassedAmount != 0");
			Assert.True(sr.FirstIndex().DataPos == gs.DataStoredPos[0], $"sr.FirstIndex().DataPos ({sr.FirstIndex().DataPos}) != gs.DataStoredPos[0] ({gs.DataStoredPos[0]})");
			Assert.True(sr.FirstIndex().QuickSeek == gs.DataPos[0] + (ulong)gs.Indexes[0].Length + 9uL, $"sr.FirstIndex().QuickSeek ({sr.FirstIndex().QuickSeek}) != gs.DataPos[0] ({gs.DataPos[0] + (ulong)gs.Indexes[0].Length + 9uL})");
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}