using System;

namespace StringDB.Reader {
	/// <summary>Defines an interaction between an IReader and any other class, or vice versa</summary>
	public interface IReaderInteraction {

		/// <summary>The string indexer for the interaction.</summary>
		string Index { get; }

		/// <summary>The position to seek to. Generally represents the starting byte for the indexer.</summary>
		ulong QuickSeek { get; }

		/// <summary>The position where the data is located at.</summary>
		ulong DataPosition { get; }
	}

	/// <inheritdoc/>
	public struct ReaderInteraction : IReaderInteraction { /// <inheritdoc/>
		public ReaderInteraction(string index, ulong dataPos = 0, ulong quickSeek = 0, uint indexSkipped = 0) {
			this.Index = index;// ?? throw new ArgumentNullException(nameof(index));
			this.QuickSeek = quickSeek;
			this.DataPosition = dataPos;
			this.IndexChainPassedAmount = indexSkipped;
		} /// <inheritdoc/>

		public string Index { get; } /// <inheritdoc/>
		public ulong QuickSeek { get; } /// <inheritdoc/>
		public ulong DataPosition { get; }

		/// <summary>The amount of times the index chain was passed to get this interaction. Seldomly used for general consumer reasons.</summary
		public uint IndexChainPassedAmount { get; }

		public static IReaderInteraction Null => new ReaderInteraction(null, 0, 0, 0);
	}
}