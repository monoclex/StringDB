using System;

namespace StringDB.Reader {

	/// <summary>Make a ReaderPair thread safe.</summary>
	internal struct ThreadSafeReaderPair : IReaderPair {

		internal ThreadSafeReaderPair(IReaderPair readerPair, object @lock) {
			this._readerPair = readerPair;
			this._lock = @lock;
		}

		/// <summary>Turn a given ReaderPair into a ThreadSafeReaderPair</summary>
		/// <param name="readerPair">The ReaderPair</param>
		/// <param name="lock">The lock to lock onto</param>
		public static ThreadSafeReaderPair FromPair(IReaderPair readerPair, object @lock)
			=> new ThreadSafeReaderPair(readerPair, @lock);

		internal IReaderPair _readerPair;
		private object _lock;

		/// <inheritdoc/>
		public long DataPosition { get => this._readerPair.DataPosition; }

		/// <inheritdoc/>
		public long Position => this._readerPair.Position;

		/// <inheritdoc/>
		public byte[] ByteArrayIndex => this._readerPair.ByteArrayIndex;

		/// <inheritdoc/>
		public string Index => this._readerPair.ByteArrayIndex.GetString();

		/// <inheritdoc/>
		public long ValueLength {
			get {
				lock (this._lock) return this._readerPair.ValueLength;
			}
		}

		/// <inheritdoc/>
		public T GetIndex<T>() {
			lock (this._lock) return this._readerPair.GetIndex<T>();
		}

		/// <inheritdoc/>
		public T GetIndexAs<T>() {
			lock (this._lock) return this._readerPair.GetIndexAs<T>();
		}
		/// <inheritdoc/>
		public Type GetIndexType() {
			lock (this._lock) return this._readerPair.GetIndexType();
		}

		/// <inheritdoc/>
		public T GetValue<T>() {
			lock (this._lock) return this._readerPair.GetValue<T>();
		}

		/// <inheritdoc/>
		public T GetValueAs<T>() {
			lock (this._lock) return this._readerPair.GetValueAs<T>();
		}

		/// <inheritdoc/>
		public Type GetValueType() {
			lock (this._lock) return this._readerPair.GetValueType();
		}

		/// <summary>A simple string form of the item.</summary>
		public override string ToString() {
			lock (this._lock) return this._readerPair.ToString();
		}
	}
}