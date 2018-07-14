using System;

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
		string Index { get; }
		
		//TODO: make a class that has GetType, Get, GetAs fucntions to remove unneeded repeating

		/// <summary>Get the proper type of the index.</summary>
		Type GetIndexType();
		
		/// <summary>Get the index as the type it was meant to be.</summary>
		/// <remarks>See GetIndexAs to try convert the value into the specified type.</remarks>
		/// <typeparam name="T">The type of the data that is stored.</typeparam>
		T GetIndex<T>();

		/// <summary>Get the index as any type the TypeHandler can handle.</summary>
		/// <typeparam name="T">The type to read the index as.</typeparam>
		T GetIndexAs<T>();

		/// <summary>Read the data stored where the value is as the type it was meant to be.</summary>
		/// <remarks>See GetValueAs to try convert the value into the specified type.</remarks>
		/// <typeparam name="T">The type of the data that is stored.</typeparam>
		T GetValue<T>();

		/// <summary>Read the data stored where the value is and ignore the type it should be, and try to convert it.</summary>
		/// <typeparam name="T">The type you want it to be.</typeparam>
		T GetValueAs<T>();

		/// <summary>Get the proper type of the value.</summary>
		Type GetValueType();

		/// <summary>Get how long the value is without reading it into memory.</summary>
		long ValueLength { get; }
	}

	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	internal struct ReaderPair : IReaderPair {

		internal ReaderPair(long dataPos, long pos, byte[] index, byte identifier, IRawReader rawReader) {
			this._identifier = identifier;
			this._rawReader = rawReader;
			this._dataPos = dataPos;
			this._index = index;
			this._pos = pos;
		}

		private IRawReader _rawReader { get; }
		internal long _dataPos;
		internal long _pos { get; }
		internal byte[] _index { get; }
		internal byte _identifier { get; }

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
		public string Index => this._index.GetString();

		/// <inheritdoc/>
		public T GetIndex<T>()
			=> this._rawReader.ReadData<T>(this._pos + sizeof(long));

		/// <inheritdoc/>
		public T GetIndexAs<T>()
			=> this._rawReader.ReadDataAs<T>(this._pos + sizeof(long), (long)this._identifier);

		/// <inheritdoc/>
		public Type GetIndexType()
			=> this._rawReader.ReadType(this._pos + sizeof(long)).Type;

		/// <inheritdoc/>
		public T GetValue<T>()
			=> this._rawReader.ReadData<T>(this._dataPos);

		/// <inheritdoc/>
		public T GetValueAs<T>()
			=> this._rawReader.ReadDataAs<T>(this._dataPos);

		/// <inheritdoc/>
		public Type GetValueType()
			=> this._rawReader.ReadType(this._dataPos).Type;

		/// <inheritdoc/>
		public long ValueLength
			=> this._rawReader.ReadLength(this._dataPos);

		/// <summary>A simple string form of the item.</summary>
		public override string ToString() =>
			$"[\"{this.Index}\", Identifier 0x{this._rawReader.ReadType(this._dataPos).Id.ToString("x2")}, \"{this.ValueLength} bytes\"]";
	}
}