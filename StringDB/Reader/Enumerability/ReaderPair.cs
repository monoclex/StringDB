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

		//TODO: documentation

		IRuntimeValue Index { get; }
		IRuntimeValue Value { get; }

		/// <summary>Get how long the value is without reading it into memory.</summary>
		long ValueLength { get; }
	}

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	public struct ReaderPair : IReaderPair {

		internal ReaderPair(long dataPos, long pos, byte[] index, byte identifier, byte indexType, IRawReader rawReader) {
			this._identifier = identifier;
			this._rawReader = rawReader;
			this._indexType = indexType;
			this._dataPos = dataPos;
			this._index = index;
			this._pos = pos;
		}

		private IRawReader _rawReader { get; }
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

		/// <inheritdoc/>
		public long ValueLength
			=> this._rawReader.ReadLength(this._dataPos);

		/// <summary>A simple string form of the item.</summary>
		public override string ToString() =>
			$"tmp"; //$"[\"{this.Index}\", Identifier 0x{this.Value.Type().Id.ToString("x2")}, \"{this.ValueLength} bytes\"]";
	}
}