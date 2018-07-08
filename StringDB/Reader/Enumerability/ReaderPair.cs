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
		internal string _strIndexCache { get; set; } = null;

		/// <summary>Get the index as a byte array instead.</summary>
		public byte[] ByteArrayIndex => this._byteIndexCache;

		/// <summary>Whatever the index is.</summary>
		public string Index => this._strIndexCache ?? (this._strIndexCache = this._dp.Index.GetString());

		/// <summary>Read the data stored at the index as the type it was meant to be.</summary>
		/// <remarks>See GetValueAs to try convert the value into the specified type.</remarks>
		/// <typeparam name="T">The type of the data that is stored.</typeparam>
		public T GetValue<T>()
			=> this._rawReader.ReadData<T>(this._dp.DataPosition);

		/// <summary>Read the data stored at the index and ignore the type it should be, and try to convert it.</summary>
		/// <typeparam name="T">The type you want it to be.</typeparam>
		public T GetValueAs<T>()
			=> this._rawReader.ReadDataAs<T>(this._dp.DataPosition);

		/// <summary>Get how long the value is without reading it into memory.</summary>
		public long ValueLength
			=> this._rawReader.ReadLength(this._dp.DataPosition);

		/// <summary>A simple string form of the item.</summary>
		/// <returns>[index, value]</returns>
		public override string ToString() =>
			$"[{this.Index}, {this.ValueLength} bytes]";
	}
}