using StringDB.Writer;
using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Tests.NullTests {
	public partial class InoperableWriterTests {
		private InoperableWriter _inopWriter = null;
		private IWriter Writer =>
			this._inopWriter ?? (this._inopWriter = new InoperableWriter());
	}
}