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

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, T1 index, T2 value);

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, KeyValuePair<T1, T2> kvp);

		/// <summary>Insert multiple items into the database.</summary>
		void InsertRange<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, IEnumerable<KeyValuePair<T1, T2>> items);

		/// <summary>Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.</summary>
		void OverwriteValue<T>(TypeHandler<T> typeHandler, IReaderPair replacePair, T newValue);

		/// <summary>Flushes the prepending data to write to the stream</summary>
		void Flush();
	}

	/// <inheritdoc/>
	public class Writer : IWriter {

		/// <summary>Create a new Writer.</summary>
		/// <param name="s">The stream</param>
		public Writer(Stream s) => this._rawWriter = new RawWriter(s);

		private readonly RawWriter _rawWriter;

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
			=> this.OverwriteValue<T>(TypeManager.GetHandlerFor<T>(), replacePair, newValue);

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, T1 index, T2 value)
			=> this.Insert<T1, T2>(typeHandlerT1, typeHandlerT2, new KeyValuePair<T1, T2>(index, value));

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, KeyValuePair<T1, T2> kvp)
			=> this.InsertRange<T1, T2>(typeHandlerT1, typeHandlerT2, kvp.AsEnumerable());

		/// <inheritdoc/>
		public void InsertRange<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, IEnumerable<KeyValuePair<T1, T2>> items)
			=> this._rawWriter.InsertRange<T1, T2>(typeHandlerT1, typeHandlerT2, items);

		/// <inheritdoc/>
		public void OverwriteValue<T>(TypeHandler<T> typeHandler, IReaderPair replacePair, T newValue) {
			var newPos = this._rawWriter.OverwriteValue<T>(typeHandler, newValue, replacePair.Value.Length(), replacePair.DataPosition, replacePair.Position + sizeof(byte));

			//TODO: update the readerpair?

			/*
			var repl = replacePair;
			while (replacePair is ThreadSafeReaderPair newRepl)
				repl = newRepl._readerPair;

			if (repl is ReaderPair rp)
				rp._dataPos = newPos;
			*/
		}

		/// <inheritdoc/>
		public void Flush()
			=> this._rawWriter.Flush();
	}
}