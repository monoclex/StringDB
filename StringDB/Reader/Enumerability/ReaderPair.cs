namespace StringDB.Reader {

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	public class ReaderPair {

		internal ReaderPair(PartDataPair dp, IRawReader rawReader) {
			this._dp = dp;
			this._rawReader = rawReader;

			this._byteIndexCache = this._dp.Index;
		}

		internal PartDataPair _dp { get; }
		private IRawReader _rawReader { get; }

		internal byte[] _byteIndexCache { get; set; }
		internal byte[] _byteValueCache { get; set; } = null;

		internal string _strIndexCache { get; set; } = null;
		internal string _strValueCache { get; set; } = null;

		public T GetValueAs<T>()
			=> this._dp.ReadAs<T>(this._rawReader);

		/// <summary>Get the index as a byte array instead.</summary>
		public byte[] ByteArrayIndex => this._byteIndexCache;

		/// <summary>Get the value as a byte array instead.</summary>
		public byte[] ByteArrayValue => this._byteValueCache ?? (this._byteValueCache = (this._dp.ReadData(this._rawReader) ?? new byte[0] { }));

		/// <summary>Get a stream of the value</summary>
		public System.IO.Stream StreamValue => this._rawReader.GetStreamOfDataAt(this._dp.DataPosition);

		/// <summary>Whatever the index is.</summary>
		public string Index => this._strIndexCache ?? (this._strIndexCache = this._dp.Index.GetString());

		/// <summary>Retrieves the value of the index. This value isn't actually fetched until you call on it, for performance reasons.</summary>
		public string Value => this._strValueCache ?? (this._strValueCache = this.ByteArrayValue.GetString());

		/// <summary>Get how long the value is without reading it into memory.</summary>
		public long ValueLength => this._dp.DataLength(this._rawReader);

		/// <summary>A simple string form of the item.</summary>
		/// <returns>[index, value]</returns>
		public override string ToString() =>
			$"[{this.Index}, {this.Value}]";
	}
}