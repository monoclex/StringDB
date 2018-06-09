using StringDB.Writer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests.NullTests {
	public partial class StreamWriterTests {
		private StreamWriter _writer = null;
		private IWriter Writer =>
			this._writer ?? (this._writer = new StreamWriter(new System.IO.MemoryStream(), DatabaseVersion.Latest, false));
	}
}