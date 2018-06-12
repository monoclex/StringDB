using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class LastIndexTest {
		[Fact]
		public void LastIndexOneByOne_String() =>
			LastIndex(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void LastIndexOneByOne_Bytes() =>
			LastIndex(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void LastIndexOneByOne_Stream() =>
			LastIndex(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void LastIndexOneByTwo_String() =>
			LastIndex(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void LastIndexOneByTwo_Bytes() =>
			LastIndex(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void LastIndexOneByTwo_Stream() =>
			LastIndex(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void LastIndexTwoByOne_String() =>
			LastIndex(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void LastIndexTwoByOne_Bytes() =>
			LastIndex(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void LastIndexTwoByOne_Stream() =>
			LastIndex(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void LastIndexTwoByTwo_String() =>
			LastIndex(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void LastIndexTwoByTwo_Bytes() =>
			LastIndex(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void LastIndexTwoByTwo_Stream() =>
			LastIndex(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void LastIndex(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			var lin = gs.Indexes.Length - 1;

			Assert.True(sr.LastIndex().Index == gs.Indexes[lin], $"sr.LastIndex().Index != gs.Indexes[lin]");
			//Assert.True(((ReaderInteraction)sr.LastIndex()).IndexChainPassedAmount != 0, $"sr.LastIndex().IndexChainPassedAmount != 0");
			Assert.True(sr.LastIndex().DataPosition == gs.DataStoredPos[lin], $"sr.LastIndex().DataPos ({sr.LastIndex().DataPosition}) != gs.DataStoredPos[{lin}] ({gs.DataStoredPos[lin]})");
			Assert.True(sr.LastIndex().QuickSeek == gs.DataPos[lin], $"sr.LastIndex().QuickSeek ({sr.LastIndex().QuickSeek}) != gs.DataPos[{lin}] ({gs.DataPos[lin]})");
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}