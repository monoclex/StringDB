using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	public class ReaderEnumerator : IEnumerator<IReaderPair> {
		internal ReaderEnumerator(IRawReader rawReader) {
			this._rawReader = rawReader;
			this._partOn = Part.Start;
		}

		private IRawReader _rawReader;
		private IPart _partOn = null;

		public IReaderPair Current => new ReaderPair(_partOn as IPartDataPair, this._rawReader);
		object IEnumerator.Current => this.Current;
		
		public bool MoveNext() {
			this._partOn = this._rawReader.ReadOn(this._partOn);

			while (this._partOn != null && (this._partOn is IPartIndexChain)) //we don't want index chains lol
				this._partOn = this._rawReader.ReadOn(this._partOn);

			return this._partOn != null;
		}

		public void Reset() => this._partOn = Part.Start;

		public void Dispose() {
			this._rawReader = null;
			this._partOn = null;
		}
	}
}