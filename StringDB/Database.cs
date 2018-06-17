using StringDB.Reader;
using StringDB.Writer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {
	public class Database : IReader, IWriter, IDisposable {
		internal Database(Stream s, bool disposeStream = false) {
			this._lock = new object();

			this._disposeStream = disposeStream;

			this._stream = s;
			this._reader = new Reader.Reader(s, this._lock);
			this._writer = new Writer.Writer(s, this._lock);
		}

		public static Database FromStream(Stream s) => new Database(s);
		public static Database FromFile(string name) => new Database(File.Open(name, FileMode.OpenOrCreate), true);

		private object _lock;

		private bool _disposeStream;
		private Stream _stream;
		private IReader _reader;
		private IWriter _writer; /// <inheritdoc/>

		//public long BytesOfStringDBOverhead() => this._reader.BytesOfStringDBOverhead();
		public IReaderPair First() => this._reader.First(); /// <inheritdoc/>
		public IEnumerator<IReaderPair> GetEnumerator() => this._reader.GetEnumerator(); /// <inheritdoc/>
		public IReaderPair GetByIndex(string index) => this._reader.GetByIndex(index ?? throw new ArgumentNullException(nameof(index))); /// <inheritdoc/>
		public IEnumerable<IReaderPair> GetMultipleByIndex(string index) => this._reader.GetMultipleByIndex(index ?? throw new ArgumentNullException(nameof(index))); /// <inheritdoc/>
		internal void NotifyInsert(ICollection<KeyValuePair<string, string>> inserts) => (this._reader as Reader.Reader).NotifyInsert(inserts ?? throw new ArgumentNullException(nameof(inserts))); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this._reader.GetEnumerator(); /// <inheritdoc/>

		public void Insert(string index, string value) {
			this._writer.Insert(index ?? throw new ArgumentNullException(nameof(index)), value ?? throw new ArgumentNullException(nameof(value)));
			this.NotifyInsert(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(index, value) });
		} /// <inheritdoc/>

		public void InsertRange(ICollection<KeyValuePair<string, string>> items) {
			this._writer.InsertRange(items ?? throw new ArgumentNullException(nameof(items)));
			this.NotifyInsert(items);
		} /// <inheritdoc/>

		public void Dispose() {
			this._writer.Dispose();
			this._writer = null;

			this._reader = null;

			this._stream.Flush();

			if (this._disposeStream)
				this._stream.Dispose();
		}
	}
}