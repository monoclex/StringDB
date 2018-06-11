using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Reader {
	/// <inheritdoc/>
	public class NewStreamReader : IReader {

		/// <summary>Create a new StreamReader.</summary>
		/// <param name="stream">The stream to read.</param>
		/// <param name="dbv">The database version to read from.</param>
		/// <param name="leaveOpen">Whether or not the stream is to be disposed after using it.</param>
		public NewStreamReader(Stream stream, DatabaseVersion dbv, bool leaveOpen) {
			this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this._dbv = dbv;
			this._leaveOpen = leaveOpen;

#if NET20 || NET35 || NET40
			this._br = new BinaryReader(this._stream, Encoding.UTF8);
#else
			this._br = new BinaryReader(this._stream, Encoding.UTF8, leaveOpen);
#endif
		}

		private Stream _stream;
		private BinaryReader _br;
		private DatabaseVersion _dbv;
		private bool _leaveOpen; /// <inheritdoc/>

		#region public methods
		public IReaderInteraction FirstIndex() => throw new NotImplementedException(); /// <inheritdoc/>

		public byte[] GetDirectValueOf(ulong dataPos) => throw new NotImplementedException(); /// <inheritdoc/>

		public byte[][] GetIndexes() => throw new NotImplementedException(); /// <inheritdoc/>

		public ulong GetOverhead() => throw new NotImplementedException(); /// <inheritdoc/>

		public IReaderChain GetReaderChain() => throw new NotImplementedException(); /// <inheritdoc/>

		public byte[] GetValueOf(IReaderInteraction r, bool doSeek = true) => this.GetValueOf(r.Index, doSeek, r.QuickSeek); /// <inheritdoc/>
		public byte[] GetValueOf(string index, bool doSeek = true, ulong quickSeek = 0) => throw new NotImplementedException(); /// <inheritdoc/>

		public byte[][] GetValuesOf(IReaderInteraction r, bool doSeek = true) => this.GetValuesOf(r.Index, doSeek, r.QuickSeek); /// <inheritdoc/>
		public byte[][] GetValuesOf(string index, bool doSeek = true, ulong quickSeek = 0) => throw new NotImplementedException(); /// <inheritdoc/>

		public IReaderInteraction IndexAfter(IReaderInteraction r, bool doSeek = true) => this.IndexAfter(r.Index, doSeek, r.QuickSeek); /// <inheritdoc/>
		public IReaderInteraction IndexAfter(string index, bool doSeek = true, ulong quickSeek = 0) => throw new NotImplementedException(); /// <inheritdoc/>

		public bool IsIndexAfter(IReaderInteraction r, bool doSeek = true) => this.IsIndexAfter(r.Index, doSeek, r.QuickSeek); /// <inheritdoc/>
		public bool IsIndexAfter(string index, bool doSeek = true, ulong quickSeek = 0) => throw new NotImplementedException(); /// <inheritdoc/>

		public IEnumerator<ReaderPair> GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex()); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex());
		#endregion

		#region IDisposable Support
		private bool disposedValue = false; /// <inheritdoc/>

		protected virtual void Dispose(bool disposing) {
			if (!this.disposedValue) {
				if (disposing && !this._leaveOpen) {
					this._stream.Dispose();

#if NET20 || NET35 || NET40
					((IDisposable)this._br).Dispose();
#else
					this._br.Dispose();
#endif
				}
				
				this.disposedValue = true;
			}
		} /// <inheritdoc/>

		public void Dispose() =>
			this.Dispose(true);
#endregion
	}
}