using StringDB.Reader;
using StringDB.Writer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {
	/// <summary>A StringDB database, used to encapsulate an IReader and an IWriter together for easy usage.</summary>
	public interface IDatabase {
		/// <summary>Cleans out the current database, and copies all of the contents of this database into the other one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.</summary>
		/// <param name="dbCleanTo">The database that will be used to insert the other database's values into</param>
		void CleanTo(Database dbCleanTo);

		/// <summary>Cleans out the database specified, and copies all of the contents of the other database into this one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.</summary>
		/// <param name="dbCleanFrom">The database to clean up</param>
		void CleanFrom(Database dbCleanFrom);
	}

	/// <inheritdoc/>
	public class Database : IReader, IWriter, IDisposable, IDatabase {
		internal Database(Stream s, bool disposeStream) {
			this._lock = new object();

			this._disposeStream = disposeStream;

			this._stream = s;
			this._reader = new Reader.Reader(s, this._lock);
			this._writer = new Writer.Writer(s, this._lock);
		}

		/// <summary>Create a new Database from a stream</summary><param name="s">The stream to be using</param><param name="disposeStream">If the stream should be disposed after we're done using it</param>
		public static Database FromStream(Stream s, bool disposeStream = false) => new Database(s, disposeStream);

		/// <summary>Create a new Database from a string name to open a file</summary><param name="name">The name of the file</param>
		public static Database FromFile(string name) => new Database(File.Open(name, FileMode.OpenOrCreate), true);

		private object _lock;

		private bool _disposeStream;
		private Stream _stream;
		private IReader _reader;
		private IWriter _writer; /// <inheritdoc/>

		public void CleanTo(Database dbCleanTo) =>
			dbCleanTo.InsertRange(FromDatabase(this)); /// <inheritdoc/>

		public void CleanFrom(Database dbCleanFrom) =>
			this.InsertRange(FromDatabase(dbCleanFrom)); /// <inheritdoc/>

		public void DrainBuffer() =>
			this._reader.DrainBuffer(); /// <inheritdoc/>

		public IEnumerator<ReaderPair> GetEnumerator() =>
			this._reader.GetEnumerator(); /// <inheritdoc/>

		IEnumerator IEnumerable.GetEnumerator() =>
			this._reader.GetEnumerator(); /// <inheritdoc/>

		public ReaderPair First() =>
			this._reader.First(); /// <inheritdoc/>

		public ReaderPair GetByIndex(string index) =>
			this._reader.GetByIndex(index ?? throw new ArgumentNullException(nameof(index))); /// <inheritdoc/>

		public IEnumerable<ReaderPair> GetMultipleByIndex(string index) =>
			this._reader.GetMultipleByIndex(index ?? throw new ArgumentNullException(nameof(index))); /// <inheritdoc/>

		public void Insert(string index, string value) =>
			this._writer.Insert(index ?? throw new ArgumentNullException(nameof(index)), value ?? throw new ArgumentNullException(nameof(value))); /// <inheritdoc/>
		
		public void Insert(KeyValuePair<string, string> kvp) {
			if (kvp.Key == null) throw new ArgumentNullException(nameof(kvp), "The key was null.");
			if (kvp.Value == null) throw new ArgumentNullException(nameof(kvp), "The value was null.");

			this._writer.Insert(kvp);
		} /// <inheritdoc/>

		public void InsertRange(IEnumerable<KeyValuePair<string, string>> items) =>
			this._writer.InsertRange(items ?? throw new ArgumentNullException(nameof(items))); /// <inheritdoc/>

		public void OverwriteValue(ReaderPair replacePair, string newValue) {
			this._writer.OverwriteValue(replacePair ?? throw new ArgumentNullException(nameof(replacePair)), newValue ?? throw new ArgumentNullException(newValue));
			this._reader.DrainBuffer();
		} /// <inheritdoc/>

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
	
	/*
	/// <inheritdoc/>
	public class ThreadSafeDatabase : IReader, IWriter, IDisposable, IDatabase {
		internal ThreadSafeDatabase(Stream s, bool disposeStream) =>
			this._db = new ThreadSafeWrapper<Database>(new Database(s, disposeStream));

		private ThreadSafeWrapper<Database> _db { get; }

		/// <summary>Create a new ThreadSafeDatabase from a stream</summary><param name="s">The stream to be using</param><param name="disposeStream">If the stream should be disposed after we're done using it</param>
		public static ThreadSafeDatabase FromStream(Stream s, bool disposeStream = false) => new ThreadSafeDatabase(s, disposeStream);

		/// <summary>Create a new ThreadSafeDatabase from a string name to open a file</summary><param name="name">The name of the file</param>
		public static ThreadSafeDatabase FromFile(string name) => new ThreadSafeDatabase(File.Open(name, FileMode.OpenOrCreate), true); /// <inheritdoc/>

		public void CleanTo(Database dbCleanTo) => this._db.WrappedObject.CleanTo(dbCleanTo); /// <inheritdoc/>
		public void CleanFrom(Database dbCleanFrom) => this._db.WrappedObject.CleanFrom(dbCleanFrom); /// <inheritdoc/>

		public ReaderPair First() => this._db.WrappedObject.First(); /// <inheritdoc/>
		public ReaderPair GetByIndex(string index) => this._db.WrappedObject.GetByIndex(index); /// <inheritdoc/>
		public IEnumerable<ReaderPair> GetMultipleByIndex(string index) => this._db.WrappedObject.GetMultipleByIndex(index); /// <inheritdoc/>
		public void DrainBuffer() => this._db.WrappedObject.DrainBuffer(); /// <inheritdoc/>
		public IEnumerator<ReaderPair> GetEnumerator() => this._db.WrappedObject.GetEnumerator(); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this._db.WrappedObject.GetEnumerator(); /// <inheritdoc/>

		public void Insert(string index, string value) => this._db.WrappedObject.Insert(index, value); /// <inheritdoc/>
		public void Insert(KeyValuePair<string, string> kvp) => this._db.WrappedObject.Insert(kvp); /// <inheritdoc/>
		public void InsertRange(IEnumerable<KeyValuePair<string, string>> items) => this._db.WrappedObject.InsertRange(items); /// <inheritdoc/>
		public void OverwriteValue(ReaderPair replacePair, string newValue) => this._db.WrappedObject.OverwriteValue(replacePair, newValue); /// <inheritdoc/>

		public void Dispose() => this._db.WrappedObject.Dispose();
	}
	*/
}