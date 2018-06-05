using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	public interface IReader : IEnumerable<KeyValuePair<string, string>> {
		string GetValueOf(IReaderInteraction r, bool doSeek = false);
		string GetValueOf(string index, bool doSeek = false, ulong quickSeek = 0);

		string[] GetValuesOf(IReaderInteraction r, bool doSeek = false);
		string[] GetValuesOf(string index, bool doSeek = false, ulong quickSeek = 0);

		bool IsIndexAfter(IReaderInteraction r, bool doSeek = false);
		bool IsIndexAfter(string index, bool doSeek = false, ulong quickSeek = 0);

		IReaderInteraction IndexAfter(IReaderInteraction r, bool doSeek = false);
		IReaderInteraction IndexAfter(string index, bool doSeek = false, ulong quickSeek = 0);

		IReaderInteraction FirstIndex();

		string[] GetIndexes();

		IReaderChain GetReaderChain();
	}
}