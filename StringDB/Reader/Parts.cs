namespace StringDB.Reader {

	internal interface IPart {

		/// <summary>The initial byte detected</summary>
		byte InitialByte { get; }

		/// <summary>position of this part</summary>
		long Position { get; }

		/// <summary>Where the next position will be</summary>
		long NextPart { get; }
	}

	internal struct Part : IPart {

		internal Part(byte initByte, long pos, long nextPart) {
			this.InitialByte = initByte;
			this.Position = pos;
			this.NextPart = nextPart;
		}

		private static Part _cacheStart = new Part(0x00, 8, 8); //cache the start for possibly better performance?
		public static Part Start => _cacheStart; /// <inheritdoc/>

		public byte InitialByte { get; } /// <inheritdoc/>
		public long Position { get; } /// <inheritdoc/>
		public long NextPart { get; }
	}

	internal struct PartIndexChain : IPart {

		internal PartIndexChain(byte initByte, long pos, long nextPart) {
			this.InitialByte = initByte;
			this.Position = pos;
			this.NextPart = nextPart;
		} /// <inheritdoc/>

		public byte InitialByte { get; } /// <inheritdoc/>
		public long Position { get; } /// <inheritdoc/>
		public long NextPart { get; }
	}

	internal struct PartDataPair : IPart {

		internal PartDataPair(byte initByte, long pos, long dataPos, byte[] indexName) {
			this.InitialByte = initByte;
			this.Position = pos;
			this.NextPart = pos + sizeof(byte) + sizeof(long) + (long)initByte;
			this._dataPos = dataPos;
			this._indexName = indexName;
		}

		private long _dataPos { get; }
		private byte[] _indexName { get; } /// <inheritdoc/>

		public byte InitialByte { get; } /// <inheritdoc/>
		public long Position { get; } /// <inheritdoc/>
		public long NextPart { get; }

		public T ReadAs<T>(IRawReader rawReader)
			=> rawReader.ReadData<T>(this.DataPosition);

		public byte[] Index
			=> this._indexName;

		public long DataPosition
			=> this._dataPos;

		public byte[] ReadData(IRawReader rawReader)
			=> rawReader.ReadDataValueAt(this.DataPosition);

		public long DataLength(IRawReader rawReader)
			=> rawReader.ReadDataValueLengthAt(this.DataPosition);
	}
}