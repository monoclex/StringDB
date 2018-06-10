using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class GetValueOfTest {
		[Fact]
		public void GetValueOfOneByOne_String() =>
			GetValueOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void GetValueOfOneByOne_Bytes() =>
			GetValueOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValueOfOneByOne_Stream() =>
			GetValueOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void GetValueOfOneByTwo_String() =>
			GetValueOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void GetValueOfOneByTwo_Bytes() =>
			GetValueOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValueOfOneByTwo_Stream() =>
			GetValueOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void GetValueOfTwoByOne_String() =>
			GetValueOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void GetValueOfTwoByOne_Bytes() =>
			GetValueOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValueOfTwoByOne_Stream() =>
			GetValueOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void GetValueOfTwoByTwo_String() =>
			GetValueOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void GetValueOfTwoByTwo_Bytes() =>
			GetValueOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void GetValueOfTwoByTwo_Stream() =>
			GetValueOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void GetValueOf(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			foreach(var j in gs.StreamDat.Data)
				foreach(var i in j) {
					if (i.Value is string)
						Assert.Equal(i.Value as string, Encoding.UTF8.GetString(sr.GetValueOf(i.Key)));
					else if (i.Value is byte[])
						Assert.Equal((byte[])i.Value, sr.GetValueOf(i.Key));
					else if (i.Value is System.IO.Stream)
						Assert.Equal((System.IO.Stream)i.Value, StreamData<object>.GetStreamOf(Encoding.UTF8.GetString(sr.GetValueOf(i.Key))));
					else throw new Exception("not");
				}
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}