using System.Collections;
using System.Collections.Generic;

namespace StringDB.Reader {

	internal class ThreadSafeReaderEnumerator : IEnumerator<IReaderPair> {

		internal ThreadSafeReaderEnumerator(IEnumerator<IReaderPair> readerEnumerator, object @lock) {
			this._readerEnumerator = readerEnumerator;
			this._lock = @lock;
		}

		private IEnumerator<IReaderPair> _readerEnumerator;
		private object _lock;

		public IReaderPair Current => ThreadSafeReaderPair.FromPair(this._readerEnumerator.Current, this._lock);

		object IEnumerator.Current => ThreadSafeReaderPair.FromPair(this._readerEnumerator.Current, this._lock);

		public void Dispose() => this._readerEnumerator.Dispose();

		public bool MoveNext() {
			lock (this._lock) return this._readerEnumerator.MoveNext();
		}

		public void Reset() {
			lock (this._lock) this._readerEnumerator.Reset();
		}
	}
}