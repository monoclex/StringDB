namespace StringDB.Reader {
	/// <summary>Some kind of interaction between an IReader and another class.</summary>
	public interface IReaderInteraction {
		/// <summary>The index of this interaction</summary>
		string Index { get; }

		/// <summary>The position to QuickSeek to for this interaction</summary>
		ulong QuickSeek { get; }

		/// <summary>The position of the data to quick seek to.</summary>
		ulong DataPos { get; }
	}

	/// <inheritdoc/>
	public struct ReaderInteraction : IReaderInteraction { /// <inheritdoc/>
		public ReaderInteraction(string index, ulong quickSeek = 0, ulong dataPos = 0) {
			this.Index = index;
			this.QuickSeek = quickSeek;
			this.DataPos = dataPos;
		}/// <inheritdoc/>

		public string Index { get; }/// <inheritdoc/>
		public ulong QuickSeek { get; }/// <inheritdoc/>
		public ulong DataPos { get; }
	}
}