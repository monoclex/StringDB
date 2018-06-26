using StringDB.Reader;
using StringDB.Writer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {
	public class Database : IReader, IWriter, IDisposable {
		internal Database(Stream s, bool disposeStream) {
			this._lock = new object();

			this._disposeStream = disposeStream;

			this._stream = s;
			this._reader = new Reader.Reader(s, this._lock);
			this._writer = new Writer.Writer(s, this._lock);
		}

		public static Database FromStream(Stream s, bool disposeStream = false) => new Database(s, disposeStream);
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
		//internal void NotifyInsert(IEnumerable<KeyValuePair<string, string>> inserts) => (this._reader as Reader.Reader).NotifyInsert(inserts ?? throw new ArgumentNullException(nameof(inserts))); /// <inheritdoc/>
		//internal void NotifyInsert(IEnumerable<IReaderPair> inserts) => (this._reader as Reader.Reader).NotifyInsert(inserts ?? throw new ArgumentNullException(nameof(inserts))); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this._reader.GetEnumerator();

		/// <summary>Cleans out the database specified, and copies all of the contents of the other database into this one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.</summary>
		/// <param name="dbCleanFrom">The database to clean up</param>
		public void CleanFrom(Database dbCleanFrom) {
			this.InsertRange(FromDatabase(dbCleanFrom));
		}

		/// <summary>Cleans out the current database, and copies all of the contents of this database into the other one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.</summary>
		/// <param name="dbCleanTo">The database that will be used to insert the other database's values into</param>
		public void CleanTo(Database dbCleanTo) {
			dbCleanTo.InsertRange(FromDatabase(this));
		} /// <inheritdoc/>

		public void Insert(string index, string value) {
			this._writer.Insert(index ?? throw new ArgumentNullException(nameof(index)), value ?? throw new ArgumentNullException(nameof(value)));
			//this.NotifyInsert(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(index, value) });
		} /// <inheritdoc/>
			
		public void Insert(KeyValuePair<string, string> kvp) {
			if (kvp.Key == null) throw new ArgumentNullException(nameof(kvp), "The key was null.");
			if (kvp.Value == null) throw new ArgumentNullException(nameof(kvp), "The value was null.");

			this._writer.Insert(kvp);
			//this.NotifyInsert(new List<KeyValuePair<string, string>>() { kvp });
		} /// <inheritdoc/>

		public void InsertRange(IEnumerable<KeyValuePair<string, string>> items) {
			this._writer.InsertRange(items ?? throw new ArgumentNullException(nameof(items)));
			//this.NotifyInsert(items);
		} /// <inheritdoc/>

		public void OverwriteValue(IReaderPair replacePair, string newValue) {
			this._writer.OverwriteValue(replacePair ?? throw new ArgumentNullException(nameof(replacePair)), newValue ?? throw new ArgumentNullException(newValue));
			this._reader.DrainBuffer();
		} /// <inheritdoc/>

		public void DrainBuffer() =>
			this._reader.DrainBuffer(); /// <inheritdoc/>

		public void Dispose() {
			this._writer.Dispose();
			this._writer = null;

			this._reader = null;

			this._stream.Flush();

			if (this._disposeStream)
				this._stream.Dispose();
		}

		private IEnumerable<KeyValuePair<string, string>> FromDatabase(Database other) {
			foreach (var i in other)
				yield return new KeyValuePair<string, string>(i.Index, i.Value);
		}
	}
}