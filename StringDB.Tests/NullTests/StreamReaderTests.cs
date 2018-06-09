using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using StringDB.Reader;

namespace StringDB.Tests.NullTests
{
    public class StreamReaderTests {
		private StreamReader _reader = null;
		private IReader Reader =>
			this._reader ?? (this._reader = new StreamReader(StreamConsts.BlankStream(), DatabaseVersion.Latest, false));

		[Fact]
		public void GetOverhead() =>
			Assert.Equal<ulong>(0uL, this.Reader.GetOverhead());

		[Fact]
		public void GetValueOf() {
			Assert.Throws<ArgumentNullException>(() => { this.Reader.GetValueOf((string)null, true); });

			Assert.Throws<ArgumentNullException>(() => { this.Reader.GetValueOf((IReaderInteraction)new ReaderInteraction(null), true); });
			Assert.Throws<ArgumentNullException>(() => { this.Reader.GetValueOf((IReaderInteraction)null); });
		}

		[Fact]
		public void GetValuesOf() {
			Assert.Throws<ArgumentNullException>(() => { this.Reader.GetValuesOf((string)null, true, 0); });

			Assert.Throws<ArgumentNullException>(() => { this.Reader.GetValuesOf((IReaderInteraction)new ReaderInteraction(null), true); });
			Assert.Throws<ArgumentNullException>(() => { this.Reader.GetValuesOf((IReaderInteraction)null); });
		}

		[Fact]
		public void GetDirectValueOf() =>
			Assert.Null(this.Reader.GetDirectValueOf(0));

		[Fact]
		public void IsIndexAfter() {
			Assert.Throws<ArgumentNullException>(() => { this.Reader.IsIndexAfter((string)null, true); });

			Assert.Throws<ArgumentNullException>(() => { this.Reader.IsIndexAfter((IReaderInteraction)new ReaderInteraction(null), true); });
			Assert.Throws<ArgumentNullException>(() => { this.Reader.IsIndexAfter((IReaderInteraction)null); });
		}

		[Fact]
		public void IndexAfter() {
			Assert.Null(this.Reader.IndexAfter((string)null, true));

			Assert.Null(this.Reader.IndexAfter((IReaderInteraction)new ReaderInteraction(null)));
			Assert.Null(this.Reader.IndexAfter((IReaderInteraction)null));
		}

		[Fact]
		public void FirstIndex() =>
			Assert.Null(this.Reader.FirstIndex());

		[Fact]
		public void GetIndexes() =>
			Assert.Null(this.Reader.GetIndexes());

		[Fact]
		public void GetReaderChain() =>
			Assert.Null(this.Reader.GetReaderChain());
	}
}
