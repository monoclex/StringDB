using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StringDB.Reader {
	//TODO: google is there a way to generate a stream from a specific portion of a filestream?

	/// <summary>Defines a reader. Use it to read out data</summary>
	public interface IReader : IEnumerable<ReaderPair> {
		/// <summary>Gets the very first element in the database</summary>
		ReaderPair First();

		/// <summary>Gets the ReaderPair responsible for a given index</summary>
		ReaderPair GetByIndex(string index);

		/// <summary>Gets the multiple ReaderPairs responsible for a given index</summary>
		IEnumerable<ReaderPair> GetMultipleByIndex(string index);

		/// <summary>Clears out the buffer. Will cause performance issues if you do it too often.</summary>
		void DrainBuffer();
	}

	/// <summary>A Reader that reads out a StringDB database file.</summary>
	public class Reader : IReader {
		internal Reader(Stream stream, object @lock = null) {
			this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this._rawReader = new RawReader(this._stream, @lock);
		}

		private Stream _stream;
		private IRawReader _rawReader;
		
		/// <inheritdoc/>
		public ReaderPair First() {
			if (this._stream.Length <= 8)
				return null;

			var p = this._rawReader.ReadOn(Part.Start);

			while (!(p is IPartDataPair))
				p = this._rawReader.ReadOn(p);
				
			return new ReaderPair(p as IPartDataPair, this._rawReader);
		} /// <inheritdoc/>

		public ReaderPair GetByIndex(string index) {
			// prevent the re-use of code

			using(var enumer = this.GetMultipleByIndex(index).GetEnumerator()) {
				if (enumer.MoveNext())
					return enumer.Current;
			}

			return null;
		} /// <inheritdoc/>

		public IEnumerable<ReaderPair> GetMultipleByIndex(string index) {
			if (this._stream.Length <= 8)
				yield break;
			
			var comparing = index.GetBytes();

			foreach (var i in this)
				if (comparing.EqualTo(i.IndexAsByteArray))
					yield return i;
		} /// <inheritdoc/>

		public IEnumerator<ReaderPair> GetEnumerator() => new ReaderEnumerator(this._rawReader); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator(); /// <inheritdoc/>

		public void DrainBuffer() =>
			this._rawReader.DrainBuffer();
	}
}