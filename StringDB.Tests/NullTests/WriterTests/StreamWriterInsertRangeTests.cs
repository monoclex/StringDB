using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.NullTests {
	public partial class StreamWriterTests {
		[Fact]
		public void InsertRange_Null() {
			Assert.Throws<ArgumentNullException>(() => { this.Writer.InsertRange((ICollection<KeyValuePair<string, string>>)null); });
			Assert.Throws<ArgumentNullException>(() => { this.Writer.InsertRange((ICollection<KeyValuePair<string, byte[]>>)null); });
			Assert.Throws<ArgumentNullException>(() => { this.Writer.InsertRange((ICollection<KeyValuePair<string, System.IO.Stream>>)null); });
		}

		[Fact]
		public void InsertRange_EmptyCollection() {
			Assert.Throws<ArgumentException>(() => { this.Writer.InsertRange(new List<KeyValuePair<string, string>>() { }); });
			Assert.Throws<ArgumentException>(() => { this.Writer.InsertRange(new List<KeyValuePair<string, byte[]>>() { }); });
			Assert.Throws<ArgumentException>(() => { this.Writer.InsertRange(new List<KeyValuePair<string, System.IO.Stream>>() { }); });
		}

		[Fact]
		public void InsertRange_CollectionHasNull() {
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>(null, null)
				});
			});
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, byte[]>>() {
					new KeyValuePair<string, byte[]>(null, null)
				});
			});
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, System.IO.Stream>>() {
					new KeyValuePair<string, System.IO.Stream>(null, null)
				});
			});
		}

		[Fact]
		public void InsertRange_CollectionHasNullValues() {
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("index_1", null)
				});
			});
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, byte[]>>() {
					new KeyValuePair<string, byte[]>("index_2", null)
				});
			});
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, System.IO.Stream>>() {
					new KeyValuePair<string, System.IO.Stream>("index_3", null)
				});
			});
		}

		[Fact]
		public void InsertRange_CollectionHasNullIndexes() {
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>(null, "value_1")
				});
			});
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, byte[]>>() {
					new KeyValuePair<string, byte[]>(null, new byte[]{ 0x76, 0x61, 0x6c, 0x75, 0x65, 0x5f, 0x32 })
				});
			});
			Assert.Throws<ArgumentNullException>(() => {
				this.Writer.InsertRange(new List<KeyValuePair<string, System.IO.Stream>>() {
					new KeyValuePair<string, System.IO.Stream>(null, StreamConsts.BlankStream())
				});
			});
		}
	}
}