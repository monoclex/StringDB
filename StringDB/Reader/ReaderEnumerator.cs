using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	/// <summary>Allows you to enumerate over an IReader efficiently.</summary>
	public class ReaderEnumerator : IEnumerator<KeyValuePair<string, string>> {
		internal ReaderEnumerator(IReader parent, IReaderInteraction start) {
			this._parent = parent;

			this._indexOn = start.Index;

			this._seekTo = 0;
			this._toSeek = start.QuickSeek;
			this._first = false;
		}

		private bool _first { get; set; }

		private string _indexOn { get; set; }
		private ulong _seekTo { get; set; }
		private ulong _toSeek { get; set; }
		private IReader _parent { get; set; }

		//TODO: Reading the value of the index is resource heavy, especially if one is only iterating over it for the indexes. Should use some kind of class to fetch the value of the index for quicker reading.
		/// <inheritdoc/>
		public KeyValuePair<string, string> Current => new KeyValuePair<string, string>(this._indexOn, this._parent.GetValueOf(this._indexOn, true, this._seekTo));

		object IEnumerator.Current => this.Current; /// <inheritdoc/>

		public bool MoveNext() {
			if (!this._first) {
				this._first = true;
				return true;
			}

			var rr = this._parent.IndexAfter(this._indexOn, true, this._seekTo);

			if (rr == null)
				return false;

			this._indexOn = rr.Index;

			this._seekTo = this._toSeek;
			this._toSeek = rr.QuickSeek;

			return true;
		} /// <inheritdoc/>

		public void Reset() {
			var rr = this._parent.FirstIndex();

			this._indexOn = rr.Index;
			this._seekTo = rr.QuickSeek;
		}

		#region IDisposable Support
		private bool disposedValue = false; /// <inheritdoc/>

		protected virtual void Dispose(bool disposing) {
			if (!this.disposedValue) {
				if (disposing) {
					//TODO: dispose managed state (managed objects).
				}

				//TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				//TODO: set large fields to null.

				this.disposedValue = true;
			}
		} /// <inheritdoc/>

		//TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ReaderEnumerator() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			//TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}