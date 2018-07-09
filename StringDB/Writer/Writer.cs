using StringDB.DBTypes;
using StringDB.Reader;

using System.Collections.Generic;
using System.IO;

namespace StringDB.Writer {

	/// <summary>Some kind of wwriter that writes stuff.</summary>
	public interface IWriter {

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(T1 index, T2 value);

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(KeyValuePair<T1, T2> kvp);

		/// <summary>Insert multiple items into the database.</summary>
		void InsertRange<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> items);

		/// <summary>Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.</summary>
		void OverwriteValue<T>(IReaderPair replacePair, T newValue);
	}

	/// <inheritdoc/>
	public class Writer : IWriter {

		/// <summary>Create a new Writer.</summary>
		/// <param name="s">The stream</param>
		public Writer(Stream s) => this._rawWriter = new RawWriter(s);

		private readonly IRawWriter _rawWriter;

		// lol if this isn't a wall of text i don't know what is

		/// <inheritdoc/>
		public void Insert<T1, T2>(T1 index, T2 value)
			=> this.Insert<T1, T2>(new KeyValuePair<T1, T2>(index, value));

		/// <inheritdoc/>
		public void Insert<T1, T2>(KeyValuePair<T1, T2> kvp)
			=> this.InsertRange<T1, T2>(kvp.AsEnumerable());

		/// <inheritdoc/>
		public void InsertRange<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> items)
			=> this._rawWriter.InsertRange<T1, T2>(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), items);

		/// <inheritdoc/>
		public void OverwriteValue<T>(IReaderPair replacePair, T newValue)
			=> this._rawWriter.OverwriteValue<T>(TypeManager.GetHandlerFor<T>(), newValue, replacePair.ValueLength, replacePair.DataPosition, replacePair.Position + sizeof(byte));
	}
}