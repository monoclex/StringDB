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
		private readonly object _lock;

		/// <inheritdoc/>
		public long DataPosition => this._readerPair.DataPosition;

		/// <inheritdoc/>
		public long Position => this._readerPair.Position;

		/// <inheritdoc/>
		public byte[] ByteArrayIndex => this._readerPair.ByteArrayIndex;

		/// <inheritdoc/>
		public string StringIndex => this._readerPair.ByteArrayIndex.GetString();

		/// <inheritdoc/>
		public IRuntimeValue Index {
			get {
				lock (this._lock) return new ThreadSafeRuntimeValue(this._readerPair.Index, this._lock);
			}
		}

		/// <inheritdoc/>
		public IRuntimeValue Value {
			get {
				lock (this._lock) return new ThreadSafeRuntimeValue(this._readerPair.Value, this._lock);
			}
		}

		/// <summary>A simple string form of the item.</summary>
		public override string ToString() {
			lock (this._lock) return this._readerPair.ToString();
		}
	}
}