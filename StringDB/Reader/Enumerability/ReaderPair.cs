namespace StringDB.Reader {

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	public interface IReaderPair {

		/// <summary>The position in the file that this ReaderPair is located at</summary>
		long Position { get; }

		/// <summary>The position of where the data is stored for this ReaderPair</summary>
		long DataPosition { get; }

		/// <summary>Get the index as a byte array instead.</summary>
		byte[] ByteArrayIndex { get; }

		/// <summary>Whatever the index is.</summary>
		string StringIndex { get; }

		/// <summary>Provides Get, GetAs, and Type functions for the Index.</summary>
		/// <remarks>It is highly advised that you use the ByteArrayIndex whenver possible.</remarks>
		IRuntimeValue Index { get; }

		/// <summary>Provides Get, GetAs, and Type functions for the Value.</summary>
		IRuntimeValue Value { get; }
	}

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	internal struct ReaderPair : IReaderPair {

		internal ReaderPair(long dataPos, long pos, byte[] index, byte identifier, byte indexType, RawReader rawReader) {
			this._identifier = identifier;
			this._rawReader = rawReader;
			this._indexType = indexType;
			this._dataPos = dataPos;
			this._index = index;
			this._pos = pos;
		}

		private RawReader _rawReader { get; }
		internal long _dataPos;
		internal long _pos { get; }
		internal byte[] _index { get; }
		internal byte _identifier { get; }
		internal byte _indexType { get; }

		/// <inheritdoc/>
		public long DataPosition {
			get => this._dataPos;
			internal set => this._dataPos = value;
		}

		/// <inheritdoc/>
		public long Position => this._pos;

		/// <inheritdoc/>
		public byte[] ByteArrayIndex => this._index;

		/// <inheritdoc/>
		public string StringIndex => this._index.GetString();

		/// <inheritdoc/>
		public IRuntimeValue Index => new RuntimeValue(this._rawReader, this._pos + sizeof(long) + sizeof(byte), this._indexType, this._identifier);

		/// <inheritdoc/>
		public IRuntimeValue Value => new RuntimeValue(this._rawReader, this._dataPos, null);

		/// <summary>A simple string form of the item.</summary>
		public override string ToString() =>
			$"[Index: {this.Index.ToString()}, Value: {this.Value.ToString()}]";
	}
}