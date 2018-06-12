using System;
using System.Collections.Generic;

namespace StringDB.Reader {
	/// <summary>A reader. It reads out a stream ( that should hold StringDB data )</summary>
	public interface IReader : IEnumerable<ReaderPair>, IDisposable {

		/// <summary>If the reader is empty</summary>
		/// <returns>True if the reader is empty and has nothing in it, false if there is stuff in it to be read.</returns>
		bool Empty();

		/// <summary>Gets the amount of bytes of overhead that StringDB is using.</summary>
		/// <returns>A ulong that represents the amount bytes StringDB is overheading.</returns>
		ulong GetOverhead();

		/// <summary>This should retrieve the value associated with an index.</summary>
		/// <param name="r">The index and quickSeek position to use.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <returns>A string that has the data in correlation to the index.</returns>
		byte[] GetValueOf(IReaderInteraction r, bool doSeek = false);

		/// <summary>This should retrieve the value associated with an index.</summary>
		/// <param name="index">The index to look for.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <param name="quickSeek">The location to seek to</param>
		/// <returns>A string that has the data in correlation to the index.</returns>
		byte[] GetValueOf(string index, bool doSeek = false, ulong quickSeek = 0);

		/// <summary>Go straight to the position specified and read out the data</summary>
		/// <param name="dataPos">The position of the data</param>
		/// <returns>A string with the data</returns>
		byte[] GetDirectValueOf(ulong dataPos);

		/// <summary>This will get all the values associated with an index</summary>
		/// <param name="r">The index and quickSeek position to use.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <returns>The pieces of data that correlate with the index</returns>
		byte[][] GetValuesOf(IReaderInteraction r, bool doSeek = false);

		/// <summary>This will get all the values associated with an index</summary>
		/// <param name="index">The index to look for.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <param name="quickSeek">The location to seek to</param>
		/// <returns>The pieces of data that correlate with the index</returns>
		byte[][] GetValuesOf(string index, bool doSeek = false, ulong quickSeek = 0);

		/// <summary>Checks if there's an index after a location.<para>Generally not recommended, as it is expensive to call this and then read the next index, and internally we don't call this anyways.</para></summary>
		/// <param name="r">The index and quickSeek position to use.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <returns>True if there is an index after the index you're looking for, false if not.</returns>
		bool IsIndexAfter(IReaderInteraction r, bool doSeek = false);

		/// <summary>Checks if there's an index after a location.<para>Generally not recommended, as it is expensive to call this and then read the next index, and internally we don't call this anyways.</para></summary>
		/// <param name="index">The index to look for.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <param name="quickSeek">The location to seek to</param>
		/// <returns>True if there is an index after the index you're looking for, false if not.</returns>
		bool IsIndexAfter(string index, bool doSeek = false, ulong quickSeek = 0);

		/// <summary>Checks if there's an index after a location.<para>The preferred version to call, because at least we store what's next for you after this. Still though, not a good idea.</para></summary>
		/// <param name="r">The index and quickSeek position to use.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <returns>An IReaderInteraction with the information if there is stuff, null if not.</returns>
		IReaderInteraction IndexAfter(IReaderInteraction r, bool doSeek = false);

		/// <summary>Checks if there's an index after a location.<para>The preferred version to call, because at least we store what's next for you after this. Still though, not a good idea.</para></summary>
		/// <param name="index">The index to look for.</param>
		/// <param name="doSeek">If the reader should seek to the quickSeek location specified.</param>
		/// <param name="quickSeek">The location to seek to</param>
		/// <returns>An IReaderInteraction with the information if there is stuff, null if not.</returns>
		IReaderInteraction IndexAfter(string index, bool doSeek = false, ulong quickSeek = 0);

		/// <summary>Finds the first index in the stream and returns it.</summary>
		/// <returns>The very first index.</returns>
		IReaderInteraction FirstIndex();

		/// <summary>Finds the very last index in the stream and returns it.</summary>
		/// <returns>The very last index.</returns>
		IReaderInteraction LastIndex();

		/// <summary>Reads the entire document to empty the indexes into a string[].</summary>
		/// <returns>A string[] containing every index.</returns>
		byte[][] GetIndexes();

		/// <summary>Gets the very last place in the IndexChain.<para>If you're using this for some reason, you shouldn't.</para></summary>
		/// <returns>The last IndexChain and IndexChainWrite to use for an IWriter.</returns>
		IReaderChain GetReaderChain();
	}
}