using StringDB.Reader;

using System.Collections;
using System.Collections.Generic;

namespace StringDB {

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
		public void CleanTo(IDatabase dbCleanTo) {
			lock (this._lock) this._other.CleanTo(dbCleanTo);
		}

		/// <inheritdoc/>
		public void CleanFrom(IDatabase dbCleanFrom) {
			lock (this._lock) this._other.CleanFrom(dbCleanFrom);
		}

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

		private static IEnumerable<IReaderPair> Wrap(IEnumerable<IReaderPair> readerPairs, object @lock) {
			foreach (var i in readerPairs)
				yield return ThreadSafeReaderPair.FromPair(i, @lock);
		}
	}
}