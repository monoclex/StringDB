using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.Reader {
	public class EnumeratorTests {
		[Fact]
		public void EnumeratorOneByOne_String() =>
			Enumerator(StreamConsts.GetBy(1, 1, StreamConsts.Type.String));
		[Fact]
		public void EnumeratorOneByOne_Bytes() =>
			Enumerator(StreamConsts.GetBy(1, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void EnumeratorOneByOne_Stream() =>
			Enumerator(StreamConsts.GetBy(1, 1, StreamConsts.Type.Stream));

		[Fact]
		public void EnumeratorOneByTwo_String() =>
			Enumerator(StreamConsts.GetBy(1, 2, StreamConsts.Type.String));
		[Fact]
		public void EnumeratorOneByTwo_Bytes() =>
			Enumerator(StreamConsts.GetBy(1, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void EnumeratorOneByTwo_Stream() =>
			Enumerator(StreamConsts.GetBy(1, 2, StreamConsts.Type.Stream));

		[Fact]
		public void EnumeratorTwoByOne_String() =>
			Enumerator(StreamConsts.GetBy(2, 1, StreamConsts.Type.String));
		[Fact]
		public void EnumeratorTwoByOne_Bytes() =>
			Enumerator(StreamConsts.GetBy(2, 1, StreamConsts.Type.Bytes));
		[Fact]
		public void EnumeratorTwoByOne_Stream() =>
			Enumerator(StreamConsts.GetBy(2, 1, StreamConsts.Type.Stream));

		[Fact]
		public void EnumeratorTwoByTwo_String() =>
			Enumerator(StreamConsts.GetBy(2, 2, StreamConsts.Type.String));
		[Fact]
		public void EnumeratorTwoByTwo_Bytes() =>
			Enumerator(StreamConsts.GetBy(2, 2, StreamConsts.Type.Bytes));
		[Fact]
		public void EnumeratorTwoByTwo_Stream() =>
			Enumerator(StreamConsts.GetBy(2, 2, StreamConsts.Type.Stream));

		private void Enumerator(GeneratedStream gs) {
			var sr = GetSR(gs.Stream);

			var p = sr.GetEnumerator();
			

			for(uint i = 0; i < gs.Indexes.Length; i++) {
				var mn = p.MoveNext();
				Assert.True(mn == (i < gs.Indexes.Length), $"p.MoveNext() ({mn}) != (i < gs.Indexes.Length) ({(i < gs.Indexes.Length)}) ({i} < {gs.Indexes.Length})))");

				if(mn) {
					var rp = p.Current;

					Assert.True(rp.Index == gs.Indexes[i], $"rp.Index ({rp.Index}) != gs.Indexes[{i}] (i = {i}) ({gs.Indexes[i]})");

					var val = gs.Values[i];
					string newVal = "";

					if (val is string)
						newVal = (string)val;
					else if (val is byte[])
						newVal = Encoding.UTF8.GetString((byte[])val);
					else if (val is System.IO.Stream)
						newVal = Encoding.UTF8.GetString(((System.IO.MemoryStream)val).ToArray());

					Assert.True(rp.Value == newVal, $"rp.Value ({rp.Value}) != gs.Values[i] ({newVal})");
				}
			}
		}

		private StreamReader GetSR(System.IO.Stream s) =>
			new StreamReader(s, DatabaseVersion.Latest, true);
	}
}