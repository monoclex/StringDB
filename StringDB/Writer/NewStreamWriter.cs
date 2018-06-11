using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Writer {

	/// <inheritdoc/>
	public class NewStreamWriter : IWriter {

		/// <summary>The rewritten version of the StreamWriter</summary>
		/// <param name="s">The stream to write to</param>
		/// <param name="dbv">The database version of the stream</param>
		/// <param name="leaveOpen">If the stream should be left open</param>
		public NewStreamWriter(Stream s, DatabaseVersion dbv = DatabaseVersion.Latest, bool leaveOpen = false) {
			this._stream = s;
			this._dbv = DatabaseVersion.Latest;
			this._leaveOpen = leaveOpen;
			
			this._bw = new BinaryWriter(this._stream, Encoding.UTF8, this._leaveOpen);
		}

		private Stream _stream;
		private BinaryWriter _bw;
		private DatabaseVersion _dbv;
		private bool _leaveOpen;

		/// <inheritdoc/>
		public void Insert(string index, string data) => throw new NotImplementedException(); /// <inheritdoc/>
		public void Insert(string index, byte[] data) => throw new NotImplementedException(); /// <inheritdoc/>
		public void Insert(string index, Stream data) => throw new NotImplementedException(); /// <inheritdoc/>
		public void InsertRange(ICollection<KeyValuePair<string, byte[]>> data) => throw new NotImplementedException(); /// <inheritdoc/>
		public void InsertRange(ICollection<KeyValuePair<string, string>> data) => throw new NotImplementedException(); /// <inheritdoc/>
		public void InsertRange(ICollection<KeyValuePair<string, Stream>> data) => throw new NotImplementedException();

		#region nitty gritty

		#endregion

		#region IDisposable Support
		/// <inheritdoc/>
		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		} /// <inheritdoc/>

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~NewStreamWriter() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		private bool disposedValue = false; // To detect redundant calls
		#endregion
	}
}