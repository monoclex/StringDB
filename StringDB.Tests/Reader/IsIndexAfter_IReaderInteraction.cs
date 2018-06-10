using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class IsIndexAfterTest_IReaderInteraction {
		[Fact]
		public void IsIndexAfterOneByOne_String() =>
			IsIndexAfter(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void IsIndexAfterOneByOne_Bytes() =>
			IsIndexAfter(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IsIndexAfterOneByOne_Stream() =>
			IsIndexAfter(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IsIndexAfterOneByTwo_String() =>
			IsIndexAfter(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void IsIndexAfterOneByTwo_Bytes() =>
			IsIndexAfter(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IsIndexAfterOneByTwo_Stream() =>
			IsIndexAfter(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void IsIndexAfterTwoByOne_String() =>
			IsIndexAfter(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void IsIndexAfterTwoByOne_Bytes() =>
			IsIndexAfter(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IsIndexAfterTwoByOne_Stream() =>
			IsIndexAfter(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IsIndexAfterTwoByTwo_String() =>
			IsIndexAfter(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void IsIndexAfterTwoByTwo_Bytes() =>
			IsIndexAfter(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IsIndexAfterTwoByTwo_Stream() =>
			IsIndexAfter(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void IsIndexAfter(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			for (uint i = 0; i < gs.Indexes.Length - 1; i++)
				Assert.True(sr.IsIndexAfter(new ReaderInteraction(gs.Indexes[i])));

			Assert.False(sr.IsIndexAfter(new ReaderInteraction(gs.Indexes[gs.Indexes.Length - 1])));
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}