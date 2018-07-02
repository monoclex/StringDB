using System.Collections;
using System.Collections.Generic;

namespace StringDB.Reader {

	/// <summary>Used to enumerate over a StringDB.</summary>
	public class ReaderEnumerator : IEnumerator<ReaderPair> {

		internal ReaderEnumerator(IRawReader rawReader) {
			this._rawReader = rawReader;
			this._partOn = Part.Start;
		}

		private IRawReader _rawReader;
		private IPart _partOn = null;

		/// <summary>What the current element is on.</summary>
		public ReaderPair Current => new ReaderPair((PartDataPair)this._partOn, this._rawReader);

		object IEnumerator.Current => this.Current;

		/// <summary>Retireves the next element in the sequence</summary>
		/// <returns>True if there is an element, false if there isn't.</returns>
		public bool MoveNext() {
			this._partOn = this._rawReader.ReadOn(this._partOn);

			while (this._partOn != null && (this._partOn is PartIndexChain)) //we don't want index chains lol
				this._partOn = this._rawReader.ReadOn(this._partOn);

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