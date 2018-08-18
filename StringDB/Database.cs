using StringDB.Reader;
using StringDB.Writer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StringDB {

	/// <summary>A StringDB database, used to encapsulate an IReader and an IWriter together for easy usage.</summary>
	public interface IDatabase : IReader, IWriter, IDisposable {

		/// <summary>Cleans out the current database, and copies all of the contents of this database into the other one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.</summary>
		/// <param name="dbCleanTo">The database that will be used to insert the other database's values into</param>
		void CleanTo(IDatabase dbCleanTo);

		/// <summary>Cleans out the database specified, and copies all of the contents of the other database into this one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.</summary>
		/// <param name="dbCleanFrom">The database to clean up</param>
		void CleanFrom(IDatabase dbCleanFrom);

		/// <summary>Replaces the current reader and writer with a thread safe reader and writer.</summary>
		void MakeThreadSafe();
	}

	/// <inheritdoc/>
	public class Database : IDatabase {

		internal Database(Stream s, bool disposeStream) {
			this._disposeStream = disposeStream;

			this._sio = s.GetStreamIO();

			this._reader = this._sio.GetReader();
			this._writer = this._sio.GetWriter();
		}

		/// <summary>Create a new Database from a stream</summary><param name="s">The stream to be using</param><param name="disposeStream">If the stream should be disposed after we're done using it</param>
		public static IDatabase FromStream(Stream s, bool disposeStream = false) => new Database(s, disposeStream);

		/// <summary>Create a new Database from a string name to open a file</summary><param name="name">The name of the file</param>
		public static IDatabase FromFile(string name) => new Database(File.Open(name, FileMode.OpenOrCreate), true);

		private readonly bool _disposeStream;
		private StreamIO _sio;
		private IReader _reader;
		private IWriter _writer;

		/// <inheritdoc/>
		public void CleanTo(IDatabase dbCleanTo) =>
			dbCleanTo.InsertRange(FromDatabase(this));

		/// <inheritdoc/>
		public void CleanFrom(IDatabase dbCleanFrom) =>
			this.InsertRange(FromDatabase(dbCleanFrom));

		/// <inheritdoc/>
		public void MakeThreadSafe() {
			var @lock = new object();

			this._reader = this._sio.GetTSReader(@lock);
			this._writer = this._sio.GetTSWriter(@lock);
		}

		#region reader

		/// <inheritdoc/>
		public IReaderPair ReadFromPosition(long position)
			=> this._reader.ReadFromPosition(position);

		/// <inheritdoc/>
		public IReaderPair First() =>
			this._reader.First();

		/// <inheritdoc/>
		public IReaderPair Get<T>(TypeHandler<T> typeHandler, T index) =>
			this._reader.Get<T>(typeHandler, index);

		/// <inheritdoc/>
		public bool TryGet<T>(TypeHandler<T> typeHandler, T index, out IReaderPair value) =>
			this._reader.TryGet<T>(typeHandler, index, out value);

		/// <inheritdoc/>
		public IEnumerable<IReaderPair> GetAll<T>(TypeHandler<T> typeHandler, T index) =>
			this._reader.GetAll<T>(typeHandler, index);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() =>
			this._reader.GetEnumerator();

		/// <inheritdoc/>
		public IEnumerator<IReaderPair> GetEnumerator() =>
			this._reader.GetEnumerator();

		/// <inheritdoc/>
		public void DrainBuffer() =>
			this._reader.DrainBuffer();

		#endregion reader

		#region writer

		/// <inheritdoc/>
		public void Flush() {
			this._writer.Flush();
			this._sio.Flush();
		}

		/// <inheritdoc/>
		public void Fill<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, T1 index, T2 value, int times)
			=> this._writer.Fill(wt1, wt2, index, value, times);

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, T1 index, T2 value)
			=> this._writer.Insert(typeHandlerT1, typeHandlerT2, index, value);

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, KeyValuePair<T1, T2> kvp)
			=> this._writer.Insert(typeHandlerT1, typeHandlerT2, kvp);

		/// <inheritdoc/>
		public void InsertRange<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, IEnumerable<KeyValuePair<T1, T2>> items)
			=> this._writer.InsertRange(typeHandlerT1, typeHandlerT2, items);

		/// <inheritdoc/>
		public void OverwriteValue<T>(TypeHandler<T> typeHandler, IReaderPair replacePair, T newValue) {
			this._writer.OverwriteValue(typeHandler, replacePair, newValue);
			this._reader.DrainBuffer();
		}

		#endregion writer

		/// <inheritdoc/>
		public void Dispose() {
			this.Flush();

			this._writer = null;
			this._reader = null;

			if (this._disposeStream)
				this._sio.Dispose();

			this._sio = null;
		}

		private static IEnumerable<KeyValuePair<byte[], Stream>> FromDatabase(IDatabase other) {
			foreach (var i in other) {
				//TODO: preserve the type of data when cleaning
				yield return new KeyValuePair<byte[], Stream>(i.RawIndex, i.Value.GetAs<Stream>());
			}
		}
	}
}