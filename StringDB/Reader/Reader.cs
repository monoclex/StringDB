using StringDB.DBTypes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StringDB.Reader {

	/// <summary>Defines a reader. Use it to read out data</summary>
	public interface IReader : IEnumerable<ReaderPair> {

		/// <summary>Gets the very first element in the database</summary>
		ReaderPair First();

		/// <summary>Gets the ReaderPair responsible for a given index</summary>
		ReaderPair GetValue<T>(T index);

		/// <summary>Attempts to get the ReaderPair</summary>
		bool TryGetValue<T>(T index, out ReaderPair value);

		/// <summary>Gets the multiple ReaderPairs responsible for a given index</summary>
		IEnumerable<ReaderPair> GetMultipleByIndex<T>(T index);

		/// <summary>Clears out the buffer. Will cause performance issues if you do it too often.</summary>
		void DrainBuffer();
	}

	/// <summary>A Reader that reads out a StringDB database file.</summary>
	public class Reader : IReader {

		internal Reader(Stream stream) {
			this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this._rawReader = new RawReader(this._stream);
		}

		private readonly Stream _stream;
		private readonly IRawReader _rawReader;

		/// <inheritdoc/>
		public ReaderPair First() {
			if (this._stream.Length <= 8)
				return default(ReaderPair); // newly created DBs have nothing

			var p = this._rawReader.ReadOn(Part.Start); // read on from the start

			do p = this._rawReader.ReadOn(p); //read on while it's not a datapair
			while (!(p is PartDataPair));

			return ((PartDataPair)p).ToReaderPair(this._rawReader);
		}

		/// <inheritdoc/>
		public ReaderPair GetValue<T>(T index) {
			// prevent the re-use of code

			using (var enumer = this.GetMultipleByIndex(index).GetEnumerator()) { // loop through the multiple found
				if (enumer.MoveNext()) {
					return enumer.Current; // return the first one of the ones we found
				}
			}

			throw new InvalidOperationException($"Unable to find the given index {index}");
		}

		/// <inheritdoc/>
		public bool TryGetValue<T>(T index, out ReaderPair value) {
			// prevent the re-use of code

			using (var enumer = this.GetMultipleByIndex(index).GetEnumerator()) { // loop through the multiple found
				if (enumer.MoveNext()) {
					value = enumer.Current;
					return true; // return the first one
				}
			}

			// couldn't find anything

			value = default(ReaderPair);
			return false;
		}

		/// <inheritdoc/>
		public IEnumerable<ReaderPair> GetMultipleByIndex<T>(T index) {
			if (this._stream.Length <= 8) // newly created DBs
				yield break;

			var typeHandler = TypeManager.GetHandlerFor<T>();

			foreach (var i in this) // for every ReaderPair we got
				if (typeHandler.Compare(index, i.GetIndexAs<T>())) // compare thats one's index with this one's index
					yield return i;
		}

		/// <inheritdoc/>
		public IEnumerator<ReaderPair> GetEnumerator() => new ReaderEnumerator(this._rawReader);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <inheritdoc/>
		public void DrainBuffer() =>
			this._rawReader.DrainBuffer();
	}
}