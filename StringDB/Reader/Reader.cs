using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StringDB.Reader {

	/// <summary>Defines a reader. Use it to read out data</summary>
	public interface IReader : IEnumerable<IReaderPair> {

		/// <summary>Gets the very first element in the database</summary>
		IReaderPair First();

		/// <summary>Gets the ReaderPair responsible for a given index</summary>
		IReaderPair Get<T>(T index);

		/// <summary>Gets the ReaderPair responsible for a given index</summary>
		IReaderPair Get<T>(TypeHandler<T> typeHandler, T index);

		/// <summary>Attempts to get the ReaderPair</summary>
		bool TryGet<T>(T index, out IReaderPair value);

		/// <summary>Attempts to get the ReaderPair</summary>
		bool TryGet<T>(TypeHandler<T> typeHandler, T index, out IReaderPair value);

		/// <summary>Gets the multiple ReaderPairs responsible for a given index</summary>
		IEnumerable<IReaderPair> GetAll<T>(T index);

		/// <summary>Gets the multiple ReaderPairs responsible for a given index</summary>
		IEnumerable<IReaderPair> GetAll<T>(TypeHandler<T> typeHandler, T index);

		/// <summary>Clears out the buffer. Will cause performance issues if you do it too often.</summary>
		void DrainBuffer();
	}

	/// <summary>A Reader that reads out a StringDB database file.</summary>
	public sealed class Reader : IReader {

		internal Reader(Stream stream, IRawReader rawReader) {
			this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this._rawReader = rawReader;
		}

		private readonly Stream _stream;
		private readonly IRawReader _rawReader;

		/// <inheritdoc/>
		public IReaderPair First() {
			if (this._stream.Length <= 8)
				return default(ReaderPair); // newly created DBs have nothing

			var p = this._rawReader.ReadOn(Part.Start); // read on from the start

			do p = this._rawReader.ReadOn(p); //read on while it's not a datapair
			while (!(p is PartDataPair));

			return ((PartDataPair)p).ToReaderPair(this._rawReader);
		}

		/// <inheritdoc/>
		public IReaderPair Get<T>(T index)
			=> Get<T>(TypeManager.GetHandlerFor<T>(), index);

		/// <inheritdoc/>
		public bool TryGet<T>(T index, out IReaderPair value)
			=> TryGet<T>(TypeManager.GetHandlerFor<T>(), index, out value);

		/// <inheritdoc/>
		public IEnumerable<IReaderPair> GetAll<T>(T index)
			=> GetAll<T>(TypeManager.GetHandlerFor<T>(), index);

		/// <inheritdoc/>
		public IReaderPair Get<T>(TypeHandler<T> typeHandler, T index) {
			// prevent the re-use of code

			foreach (var i in this.GetAll(typeHandler, index)) // loop through the multiple found
				return i; // return the first one of the ones we found

			throw new InvalidOperationException($"Unable to find the given index {index}");
		}

		/// <inheritdoc/>
		public bool TryGet<T>(TypeHandler<T> typeHandler, T index, out IReaderPair value) {
			// prevent the re-use of code

			using (var enumer = this.GetAll(typeHandler, index).GetEnumerator()) { // loop through the multiple found
				if (enumer.MoveNext()) {
					value = enumer.Current;
					return true; // return the first one
				}
			}

			// couldn't find anything

			value = null;
			return false;
		}

		/// <inheritdoc/>
		public IEnumerable<IReaderPair> GetAll<T>(TypeHandler<T> typeHandler, T index) {
			if (this._stream.Length <= 8) // newly created DBs
				yield break;

			foreach (var i in this) // for every ReaderPair we got
				if (typeHandler.Compare(index, i.Index.GetAs<T>())) // compare thats one's index with this one's index
					yield return i;
		}

		/// <inheritdoc/>
		public IEnumerator<IReaderPair> GetEnumerator() => new ReaderEnumerator(this._rawReader);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <inheritdoc/>
		public void DrainBuffer() =>
			this._rawReader.DrainBuffer();
	}
}