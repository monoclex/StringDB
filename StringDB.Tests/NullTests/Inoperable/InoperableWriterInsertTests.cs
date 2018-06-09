using System;
using System.Collections.Generic;
using System.Text;
using StringDB.Inoperable;
using StringDB.Writer;
using Xunit;

namespace StringDB.Tests.NullTests {
	public partial class InoperableWriterTests {
		[Fact]
		public void Insert_AllNull() {
			Assert.Throws<InoperableException>(() => { this.Writer.Insert(null, (string)null); });
			Assert.Throws<InoperableException>(() => { this.Writer.Insert(null, (byte[])null); });
			Assert.Throws<InoperableException>(() => { this.Writer.Insert(null, (System.IO.Stream)null); });
		}

		[Fact]
		public void Insert_IndexesNull() {
			Assert.Throws<InoperableException>(() => { this.Writer.Insert(null, "E"); });
			Assert.Throws<InoperableException>(() => { this.Writer.Insert(null, new byte[] { 0x68, 0x69 }); });
			Assert.Throws<InoperableException>(() => { this.Writer.Insert(null, (System.IO.Stream)new System.IO.MemoryStream()); });
		}

		[Fact]
		public void Insert_ValuesNull() {
			Assert.Throws<InoperableException>(() => { this.Writer.Insert("demo_1", (string)null); });
			Assert.Throws<InoperableException>(() => { this.Writer.Insert("demo_2", (byte[])null); });
			Assert.Throws<InoperableException>(() => { this.Writer.Insert("demo_3", (System.IO.Stream)null); });
		}
	}
}