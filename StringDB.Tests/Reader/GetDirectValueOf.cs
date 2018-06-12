using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace StringDB.Tests.Reader {
	public class GetDirectValueOfTest {
		[Fact]
		public void GetDirectValueOfOneByOne_String() =>
			GetDirectValueOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void GetDirectValueOfOneByOne_Bytes() =>
			GetDirectValueOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void GetDirectValueOfOneByOne_Stream() =>
			GetDirectValueOf(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void GetDirectValueOfOneByTwo_String() =>
			GetDirectValueOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void GetDirectValueOfOneByTwo_Bytes() =>
			GetDirectValueOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void GetDirectValueOfOneByTwo_Stream() =>
			GetDirectValueOf(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void GetDirectValueOfTwoByOne_String() =>
			GetDirectValueOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void GetDirectValueOfTwoByOne_Bytes() =>
			GetDirectValueOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void GetDirectValueOfTwoByOne_Stream() =>
			GetDirectValueOf(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void GetDirectValueOfTwoByTwo_String() =>
			GetDirectValueOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void GetDirectValueOfTwoByTwo_Bytes() =>
			GetDirectValueOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void GetDirectValueOfTwoByTwo_Stream() =>
			GetDirectValueOf(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void GetDirectValueOf(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			foreach (var j in gs.StreamDat.Data)
				foreach (var i in j) {
					int tmp;
					var dp = ( (tmp = Array.IndexOf(gs.Indexes, i.Key)) >= 0 ?gs.DataPos[tmp] : 0);

					using (var _sw = new System.IO.BinaryReader(gs.Stream, Encoding.UTF8, true)) {
						_sw.BaseStream.Seek((long)dp, System.IO.SeekOrigin.Begin);
						_sw.ReadByte();

						dp = _sw.ReadUInt64();

						if (i.Value is string)
							Assert.Equal(i.Value as string, Encoding.UTF8.GetString(sr.GetDirectValueOf(dp)));
						else if (i.Value is byte[])
							Assert.Equal((byte[])i.Value, sr.GetDirectValueOf(dp));
						else if (i.Value is System.IO.Stream)
							GeneratedStream.CompareStreams((System.IO.Stream)i.Value, StreamData<object>.GetStreamOf(Encoding.UTF8.GetString(sr.GetDirectValueOf(dp))));
						else throw new Exception("not");

					}
				}
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}