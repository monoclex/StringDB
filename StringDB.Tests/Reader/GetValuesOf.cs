using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class GetValuesOfTest {
		[Fact]
		public void GetValuesOfOneByOne_String() =>
			GetValuesOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void GetValuesOfOneByOne_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValuesOfOneByOne_Stream() =>
			GetValuesOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void GetValuesOfOneByTwo_String() =>
			GetValuesOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void GetValuesOfOneByTwo_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValuesOfOneByTwo_Stream() =>
			GetValuesOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void GetValuesOfTwoByOne_String() =>
			GetValuesOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void GetValuesOfTwoByOne_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValuesOfTwoByOne_Stream() =>
			GetValuesOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void GetValuesOfTwoByTwo_String() =>
			GetValuesOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void GetValuesOfTwoByTwo_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValuesOfTwoByTwo_Stream() =>
			GetValuesOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));
		
		[Fact]
		public void Dupe_GetValuesOfOneByOne_String() =>
			GetValuesOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.String, true));
		[Fact]
		public void Dupe_GetValuesOfOneByOne_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes, true));
		[Fact]
		public void Dupe_GetValuesOfOneByOne_Stream() =>
			GetValuesOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream, true));

		[Fact]
		public void Dupe_GetValuesOfOneByTwo_String() =>
			GetValuesOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.String, true));
		[Fact]
		public void Dupe_GetValuesOfOneByTwo_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes, true));
		[Fact]
		public void Dupe_GetValuesOfOneByTwo_Stream() =>
			GetValuesOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream, true));

		[Fact]
		public void Dupe_GetValuesOfTwoByOne_String() =>
			GetValuesOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.String, true));
		[Fact]
		public void Dupe_GetValuesOfTwoByOne_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes, true));
		[Fact]
		public void Dupe_GetValuesOfTwoByOne_Stream() =>
			GetValuesOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream, true));

		[Fact]
		public void Dupe_GetValuesOfTwoByTwo_String() =>
			GetValuesOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.String, true));
		[Fact]
		public void Dupe_GetValuesOfTwoByTwo_Bytes() =>
			GetValuesOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes, true));
		[Fact]
		public void Dupe_GetValuesOfTwoByTwo_Stream() =>
			GetValuesOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream, true));

		private void GetValuesOf(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			var vals = sr.GetValuesOf(gs.Indexes[0]);

			for (uint i = 0; i < vals.Length; i++)
				if(gs.Values[i] is string)
					Assert.Equal(Encoding.UTF8.GetString(vals[i]), gs.Values[i]);
				else if (gs.Values[i] is byte[])
					Assert.Equal(vals[i], gs.Values[i]);
				else if (gs.Values[i] is System.IO.Stream)
					GeneratedStream.CompareStreams(gs.Values[i] as System.IO.Stream, StreamData<object>.GetStreamOf(Encoding.UTF8.GetString(vals[i])));
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}
