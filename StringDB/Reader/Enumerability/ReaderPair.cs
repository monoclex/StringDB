namespace StringDB.Reader {

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	public struct ReaderPair {

		internal ReaderPair(long dataPos, long pos, byte[] index, IRawReader rawReader) {
			this._rawReader = rawReader;
			this._dataPos = dataPos;
			this._index = index;
			this._pos = pos;
		}

		private IRawReader _rawReader { get; }
		internal long _dataPos { get; }
		internal long _pos { get; }
		internal byte[] _index { get; }

		/// <summary>Get the index as a byte array instead.</summary>
		public byte[] ByteArrayIndex => this._index;

		/// <summary>Whatever the index is.</summary>
		public string Index => this._index.GetString();

		/// <summary>Get the index as any type the TypeHandler can handle.</summary>
		/// <typeparam name="T">The type to read the index as.</typeparam>
		public T GetIndexAs<T>()
			=> this._rawReader.ReadDataAs<T>(this._pos + sizeof(long));

		/// <summary>Read the data stored at the index as the type it was meant to be.</summary>
		/// <remarks>See GetValueAs to try convert the value into the specified type.</remarks>
		/// <typeparam name="T">The type of the data that is stored.</typeparam>
		public T GetValue<T>()
			=> this._rawReader.ReadData<T>(this._dataPos);

		/// <summary>Read the data stored at the index and ignore the type it should be, and try to convert it.</summary>
		/// <typeparam name="T">The type you want it to be.</typeparam>
		public T GetValueAs<T>()
			=> this._rawReader.ReadDataAs<T>(this._dataPos);

		/// <summary>Get how long the value is without reading it into memory.</summary>
		public long ValueLength
			=> this._rawReader.ReadLength(this._dataPos);

		/// <summary>A simple string form of the item.</summary>
		public override string ToString() =>
			$"[\"{this.Index}\", Identifier 0x{this._rawReader.ReadType(this._dataPos).Id.ToString("x2")}, \"{this.ValueLength} bytes\"]";
	}
}