using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class IndexesTest {
		[Fact]
		public void IndexesOneByOne_String() =>
			Indexes(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void IndexesOneByOne_Bytes() =>
			Indexes(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexesOneByOne_Stream() =>
			Indexes(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IndexesOneByTwo_String() =>
			Indexes(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void IndexesOneByTwo_Bytes() =>
			Indexes(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexesOneByTwo_Stream() =>
			Indexes(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void IndexesTwoByOne_String() =>
			Indexes(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void IndexesTwoByOne_Bytes() =>
			Indexes(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexesTwoByOne_Stream() =>
			Indexes(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void IndexesTwoByTwo_String() =>
			Indexes(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void IndexesTwoByTwo_Bytes() =>
			Indexes(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void IndexesTwoByTwo_Stream() =>
			Indexes(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void Indexes(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			var indxes = sr.GetIndexes();

			for (var i = 0u; i < gs.Indexes.Length; i++)
				Assert.True(gs.Indexes[i] == Encoding.UTF8.GetString(indxes[i]), $"gs.Indexes[i] ({gs.Indexes[i]}) != indxes[i] ({Encoding.UTF8.GetString(indxes[i])})");
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}