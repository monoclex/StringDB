using StringDB.DBTypes;
using StringDB.Reader;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StringDB {

	/// <summary>Uses a lock before doing any action to add thread safety</summary>
	public class ThreadSafeDatabase : IDatabase {

		internal ThreadSafeDatabase(IDatabase other) {
			this._other = other;
			this._lock = new object();
		}

		/// <summary>Create a database that is thread safe by locking onto an object before any action</summary>
		/// <param name="db">The database to make thread-safe</param>
		/// <returns>A ThreadSafeDatabase</returns>
		public static ThreadSafeDatabase FromDatabase(IDatabase db) => new ThreadSafeDatabase(db);

		private readonly object _lock;
		private IDatabase _other;

		/// <inheritdoc/>
		public IReaderPair Get<T>(T index) {
			lock (this._lock) return this._other.Get<T>(index);
		}

		/// <inheritdoc/>
		public bool TryGet<T>(T index, out IReaderPair value) {
			lock (this._lock) return this._other.TryGet<T>(index, out value);
		}

		/// <inheritdoc/>
		public IEnumerable<IReaderPair> GetAll<T>(T index) {
			lock (this._lock) return this._other.GetAll<T>(index);
		}

		/// <inheritdoc/>
		public IReaderPair Get<T>(TypeHandler<T> typeHandler, T index) {
			lock (this._lock) return this._other.Get<T>(typeHandler, index);
		}

		/// <inheritdoc/>
		public bool TryGet<T>(TypeHandler<T> typeHandler, T index, out IReaderPair value) {
			lock (this._lock) return this._other.TryGet<T>(typeHandler, index, out value);
		}

		/// <inheritdoc/>
		public IEnumerable<IReaderPair> GetAll<T>(TypeHandler<T> typeHandler, T index) {
			lock (this._lock) return this._other.GetAll<T>(typeHandler, index);
		}

		/// <inheritdoc/>
		public IEnumerator<IReaderPair> GetEnumerator() {
			lock (this._lock) return new ThreadSafeReaderEnumerator(this._other.GetEnumerator(), this._lock);
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() {
			lock (this._lock) return new ThreadSafeReaderEnumerator(this._other.GetEnumerator(), this._lock);
		}

		/// <inheritdoc/>
		public IReaderPair First() {
			lock (this._lock) return this._other.First();
		}

		/// <inheritdoc/>
		public void Insert<T1, T2>(T1 index, T2 value) {
			lock (this._lock) this._other.Insert<T1, T2>(index, value);
		}

		/// <inheritdoc/>
		public void Insert<T1, T2>(KeyValuePair<T1, T2> kvp) {
			lock (this._lock) this._other.Insert<T1, T2>(kvp);
		}

		/// <inheritdoc/>
		public void InsertRange<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> items) {
			lock (this._lock) this._other.InsertRange<T1, T2>(items);
		}

		/// <inheritdoc/>
		public void OverwriteValue<T>(IReaderPair replacePair, T newValue) {
			lock (this._lock) this._other.OverwriteValue<T>(replacePair, newValue);
		}

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, T1 index, T2 value) {
			lock (this._lock) this._other.Insert<T1, T2>(typeHandlerT1, typeHandlerT2, index, value);
		}

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, KeyValuePair<T1, T2> kvp) {
			lock (this._lock) this._other.Insert<T1, T2>(typeHandlerT1, typeHandlerT2, kvp);
		}

		/// <inheritdoc/>
		public void InsertRange<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, IEnumerable<KeyValuePair<T1, T2>> items) {
			lock (this._lock) this._other.InsertRange<T1, T2>(typeHandlerT1, typeHandlerT2, items);
		}

		/// <inheritdoc/>
		public void OverwriteValue<T>(TypeHandler<T> typeHandler, IReaderPair replacePair, T newValue) {
			lock (this._lock) this._other.OverwriteValue<T>(typeHandler, replacePair, newValue);
		}

		/// <inheritdoc/>
		public void CleanTo(IDatabase dbCleanTo) => dbCleanTo.InsertRange(Wrap(this, this._lock));

		/// <inheritdoc/>
		public void CleanFrom(IDatabase dbCleanFrom) => this.InsertRange(Wrap(dbCleanFrom, this._lock));

		/// <inheritdoc/>
		public void DrainBuffer() {
			lock (this._lock) this._other.DrainBuffer();
		}

		/// <inheritdoc/>
		public void Dispose() {
			lock (this._lock) {
				this._other.Dispose();
				this._other = null;
			}
		}

		private static IEnumerable<KeyValuePair<byte[], Stream>> Wrap(IEnumerable<IReaderPair> readerPairs, object @lock) {
			foreach (var i in readerPairs) {
				var t = ThreadSafeReaderPair.FromPair(i, @lock);
				yield return new KeyValuePair<byte[], Stream>(t.ByteArrayIndex, t.GetValueAs<Stream>());
			}
		}
	}
}