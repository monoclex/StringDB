using System.Collections;
using System.Collections.Generic;

namespace StringDB.Reader {

	/// <summary>Used to enumerate over a StringDB.</summary>
	internal class ReaderEnumerator : IEnumerator<IReaderPair> {

		internal ReaderEnumerator(RawReader rawReader) {
			this._rawReader = rawReader;
			this._partOn = Part.Start;
		}

		private RawReader _rawReader;
		private IPart _partOn;

		/// <summary>What the current element is on.</summary>
		public IReaderPair Current => ((PartDataPair)this._partOn).ToReaderPair(this._rawReader);

		object IEnumerator.Current => this.Current;

		/// <summary>Retireves the next element in the sequence</summary>
		/// <returns>True if there is an element, false if there isn't.</returns>
		public bool MoveNext() {
			do this._partOn = this._rawReader.ReadOn(this._partOn); // read the data
			while (this._partOn != null && (this._partOn is PartIndexChain)); // as long as it isn't null or an index chain

			return this._partOn != null;
		}

		/// <summary>Resets it back to the start.</summary>
		public void Reset() => this._partOn = Part.Start;

		/// <summary>Removes references to the raw reader and the part we're on.</summary>
		public void Dispose() {
			this._rawReader = null;
			this._partOn = null;
		}
	}
}