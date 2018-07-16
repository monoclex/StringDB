using System;

namespace StringDB.Reader {

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	public interface IReaderPair : IDisposable {

		/// <summary>The position in the file that this ReaderPair is located at</summary>
		long Position { get; }

		/// <summary>The position of where the data is stored for this ReaderPair</summary>
		long DataPosition { get; }

		/// <summary>Gets the raw value stored in the index.</summary>
		byte[] RawIndex { get; }

		/// <summary>Provides Get, GetAs, and Type functions for the Index.</summary>
		/// <remarks>It is highly advised that you use the ByteArrayIndex whenver possible.</remarks>
		IRuntimeValue Index { get; }

		/// <summary>Provides Get, GetAs, and Type functions for the Value.</summary>
		IRuntimeValue Value { get; }
	}

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	internal struct ReaderPair : IReaderPair {

		internal ReaderPair(long dataPos, long pos, byte[] index, byte identifier, byte indexType, IRawReader rawReader) {
			this._indexCache = null;
			this._valueCache = null;

			this._identifier = identifier;
			this._rawReader = rawReader;
			this._indexType = indexType;
			this._dataPos = dataPos;
			this._index = index;
			this._pos = pos;
		}

		private IRawReader _rawReader;
		internal long _dataPos;
		internal long _pos { get; }
		internal byte[] _index { get; }
		internal byte _identifier { get; }
		internal byte _indexType { get; }

		private IRuntimeValue _indexCache;
		private IRuntimeValue _valueCache;

		/// <inheritdoc/>
		public long DataPosition {
			get => this._dataPos;
			internal set => this._dataPos = value;
		}

		/// <inheritdoc/>
		public long Position => this._pos;

		/// <inheritdoc/>
		public byte[] RawIndex => this._index;

		/// <inheritdoc/>
		public IRuntimeValue Index => (this._indexCache ?? (_indexCache = new RuntimeValue(this._rawReader, this._pos + sizeof(long) + sizeof(byte), this._indexType, this._identifier)));

		/// <inheritdoc/>
		public IRuntimeValue Value => (this._valueCache ?? (_valueCache = new RuntimeValue(this._rawReader, this._dataPos, null)));

		/// <summary>A simple string form of the item.</summary>
		public override string ToString() =>
			$"[Index: {this.Index.ToString()}, Value: {this.Value.ToString()}]";

		public void Dispose() {
			this._indexCache?.Dispose();
			this._valueCache?.Dispose();

			this._rawReader = null;

			GC.SuppressFinalize(this);
		}
	}
}