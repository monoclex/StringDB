using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	/// <summary>A result recieved when reading through a StringDB</summary>
	public interface IReadResult {

		/// <summary>The ulong position to quick seek to to retrieve this same value again</summary>
		ulong QuickSeekToThis { get; }

		/// <summary>The next position to quick seek to to retrieve the next value</summary>
		ulong QuickSeekToNext { get; }

		/// <summary>The type of read result.</summary>
		ResultType Type { get;}
	}

	/// <summary>A enum to simplify the two types of reading results</summary>
	public enum ResultType {
		/// <summary>The end of the strem has been reached - there is no more to do.</summary>
		EndOfStream,

		/// <summary>A data pair - correlates an index with a value</summary>
		DataPair,

		/// <summary>An index chain - correlates the current set of indexes with the next set of indexes</summary>
		IndexChain
	}

	/// <summary>The specific type for an EndOfStream result</summary>
	public interface IReadEndOfStream : IReadResult { }

	/// <summary>The specific type for an index chain result</summary>
	public interface IReadIndexChain : IReadResult { }

	/// <summary>The specific type for a data pair result</summary>
	public interface IReadDataPair : IReadResult {

		/// <summary>The length of the name of the data pair</summary>
		byte IndexerLength { get; }

		/// <summary>The position to get the name of the data pair</summary>
		ulong IndexerPosition { get; }

		/// <summary>The position of the value stored</summary>
		ulong DataPosition { get; }

		/// <summary>Retrieve the name of the index</summary>
		byte[] IndexName { get; }
	}



	/// <inheritdoc/>
	public struct ReadEndOfStream : IReadEndOfStream {
		// /// <summary>We have reached the end of the stream. Are you really trying to use any kind of data at this point?</summary>
		//public ReadEndOfStream() { }

		/// <inheritdoc/>
		public ulong QuickSeekToThis => throw new NotImplementedException();

		/// <inheritdoc/>
		public ulong QuickSeekToNext => throw new NotImplementedException();

		/// <inheritdoc/>
		public ResultType Type => ResultType.EndOfStream;
	}

	/// <inheritdoc/>
	public struct ReadIndexChain : IReadIndexChain {
		/// <summary>Create a new ReadIndexChain</summary>
		/// <param name="quickSeekToThis">The position to quickly seek to this</param>
		/// <param name="quickSeekToNext">The position to quickly seek to the next IReadResult</param>
		public ReadIndexChain(ulong quickSeekToThis, ulong quickSeekToNext) {
			this.QuickSeekToThis = quickSeekToThis;
			this.QuickSeekToNext = quickSeekToNext;
		}

		/// <inheritdoc/>
		public ulong QuickSeekToThis { get; }

		/// <inheritdoc/>
		public ulong QuickSeekToNext { get; }

		/// <inheritdoc/>
		public ResultType Type => ResultType.IndexChain;
	}

	/// <inheritdoc/>
	public struct ReadDataPair : IReadDataPair {
		/// <summary>Create a new ReadDataPair</summary>
		/// <param name="quickSeekToThis"></param>
		/// <param name="quickSeekToNext"></param>
		/// <param name="indexerLength"></param>
		/// <param name="indexerPosition"></param>
		/// <param name="dataPosition"></param>
		public ReadDataPair(ulong quickSeekToThis, ulong quickSeekToNext, byte indexerLength, ulong indexerPosition, ulong dataPosition, byte[] indexName) {
			this.QuickSeekToThis = quickSeekToThis;
			this.QuickSeekToNext = quickSeekToNext;
			this.IndexerLength = indexerLength;
			this.IndexerPosition = indexerPosition;
			this.DataPosition = dataPosition;
			this.IndexName = indexName;
		}

		/// <inheritdoc/>
		public byte IndexerLength { get; }

		/// <inheritdoc/>
		public ulong IndexerPosition { get; }

		/// <inheritdoc/>
		public ulong DataPosition { get; }

		/// <inheritdoc/>
		public ulong QuickSeekToThis { get; }

		/// <inheritdoc/>
		public ulong QuickSeekToNext { get; }

		/// <inheritdoc/>
		public ResultType Type => ResultType.DataPair;

		/// <inheritdoc/>
		public byte[] IndexName { get; }
	}
}
