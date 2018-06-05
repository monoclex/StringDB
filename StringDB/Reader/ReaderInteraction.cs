using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	public interface IReaderInteraction {
		string Index { get; }
		ulong QuickSeek { get; }
		ulong DataPos { get; }
	}

	public struct ReaderInteraction : IReaderInteraction {
		public ReaderInteraction(string index, ulong quickSeek = 0, ulong dataPos = 0) {
			this.Index = index;
			this.QuickSeek = quickSeek;
			this.DataPos = dataPos;
		}

		public string Index { get; }
		public ulong QuickSeek { get; }
		public ulong DataPos { get; }
	}
}