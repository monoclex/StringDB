using StringDB.DBTypes;

using System;
using System.IO;

namespace StringDB.Reader {

	internal interface IRawReader {

		IPart ReadAt(long pos);

		IPart ReadOn(IPart previous);

		T ReadData<T>(long pos, ITypeHandler typeHandlerReadWith);

		T ReadData<T>(long pos, long len, ITypeHandler typeHandlerReadWith);

		T ReadDataAs<T>(long pos, ITypeHandler typeHandlerReadWith);

		T ReadDataAs<T>(long pos, long len, ITypeHandler typeHandlerReadWith);

		long ReadLength(long pos);

		ITypeHandler ReadType(long pos, ITypeHandler typeHandlerReadWith, byte? specifyType = null);

		void DrainBuffer();
	}

	internal class RawReader : IRawReader {

		internal RawReader(Stream s) {
			this._stream = s;
			this._br = new BinaryReader(s);
		}

		private readonly Stream _stream;
		private readonly BinaryReader _br;

		private const int BufferSize = 0x1000; //1MiB
		private const int MinusBufferSize = -1 - BufferSize;

		private long _bufferReadPos = MinusBufferSize; //the position we read the buffer at in the filestream
		private int _bufferPos = MinusBufferSize; //the position within the buffer
		private byte[] _bufferRead = new byte[BufferSize];

		private byte[] _oneByteBuffer = new byte[1];

		public IPart ReadAt(long pos) {
#if THREAD_SAFE
			lock (_lock) {
#endif
			BufferSeek(pos);

			var p = this.ReadBytes(9); // read the length, and the data pos
			var importantByte = this._bufferRead[p]; // store the index type incase the buffer updates
			var intVal = BitConverter.ToInt64(this._bufferRead, p + 1); // use BitConverter to get it as a long

			if (importantByte == Consts.IndexSeperator) { // if it's an index seperator
				return
					intVal == 0 ? // if it goes to 0, we know we've hit the end of the DB.
					(IPart)null
					: new PartIndexChain(importantByte, pos, intVal);
			} else {
				if (importantByte == Consts.NoIndex) return null; // if the index length is 0, we know we've probably hit the end of the DB as well.

				var val_pos = this.ReadBytes(importantByte); // read the length of the index
				var val = new byte[importantByte];

				for (var i = 0; i < val.Length; i++) // loop through the buffer and read bytes ( safe because buffer is larger then Consts.MaxLength )
					val[i] = this._bufferRead[val_pos + i];

				return new PartDataPair(importantByte, pos, intVal, val);
			}
#if THREAD_SAFE
			}
#endif
		}

		public IPart ReadOn(IPart previous) => // only read on if the next part can be found
			previous.NextPart != 0 ?
				this.ReadAt(previous.NextPart)
				: null;

		public T ReadData<T>(long pos, ITypeHandler typeHandlerReadWith) {
			if (typeof(T) != typeHandlerReadWith.Type) throw new Exception($"<T> and the TypeHandlerType do not match.");

			var type = ReadType(pos, null, (byte?)null); // get the proper type handler

			// throw an exception if we're reading the wrong type
			//TODO: custom exception
			if (type.Type != typeof(T)) throw new Exception($"The data you are trying to read is not of type {typeof(T)}, it is of type {type.Type}");

			// read it properly
			return (typeHandlerReadWith as TypeHandler<T>).Read(this._br);
		}

		public T ReadData<T>(long pos, long len, ITypeHandler typeHandlerReadWith) {
			if (typeof(T) != typeHandlerReadWith.Type) throw new Exception($"<T> and the TypeHandlerType do not match.");

			var type = ReadType(pos, null, (byte?)null); // get the proper type handler

			// throw an exception if we're reading the wrong type
			//TODO: custom exception
			if (type.Type != typeof(T)) throw new Exception($"The data you are trying to read is not of type {typeof(T)}, it is of type {type.Type}");

			// read it properly
			return (typeHandlerReadWith as TypeHandler<T>).Read(this._br, len);
		}

		public T ReadDataAs<T>(long pos, ITypeHandler typeHandlerReadWith) {
			if (typeof(T) != typeHandlerReadWith.Type) throw new Exception($"<T> and the TypeHandlerType do not match.");

			this._stream.Seek(pos); // seek to the data and ignore the type identifier
			this._stream.Read(this._oneByteBuffer, 0, 1);

			return (typeHandlerReadWith as TypeHandler<T>).Read(this._br);
		}

		public T ReadDataAs<T>(long pos, long len, ITypeHandler typeHandlerReadWith) {
			if (typeof(T) != typeHandlerReadWith.Type) throw new Exception($"<T> and the TypeHandlerType do not match.");

			this._stream.Seek(pos); // seek to the data and ignore the type identifier
			this._stream.Read(this._oneByteBuffer, 0, 1);

			return (typeHandlerReadWith as TypeHandler<T>).Read(this._br, len);
		}

		private TypeHandler<T> GetTypeHandlerAs<T>(long pos) {
			this._stream.Seek(pos); // seek to the data and ignore the type identifier
			this._stream.Read(this._oneByteBuffer, 0, 1);

			return (TypeManager.GetHandlerFor<T>() as TypeHandler<T>); // get the type handler for T
		}

		public long ReadLength(long pos) {
			this._stream.Seek(pos); // seek to the data and ignore the type
			this._stream.Read(this._oneByteBuffer, 0, 1);

			return TypeHandlerLengthManager.ReadLength(this._br); // read the length of the data
		}

		public ITypeHandler ReadType(long pos, ITypeHandler typeHandlerReadWith, byte? specifyType) {
			if (specifyType == null) {
				this._stream.Seek(pos); // seek to the position
				byte b;
				return (b = this._br.ReadByte()) == typeHandlerReadWith?.Id ?
					typeHandlerReadWith
					: TypeManager.GetHandlerFor(b); // get the handler for the type of data
			} else return TypeManager.GetHandlerFor((byte)specifyType);
		}

		public void DrainBuffer() {
			this._bufferPos = MinusBufferSize;
			this._bufferReadPos = MinusBufferSize;
			//we don't clear the actual byte[] buffer because that'll be done when we try to read it
		}

		//heavily optimized method of reading bytes with an internal byte[] cache
		private int ReadBytes(int amt) {
			if (this._bufferPos + amt >= BufferSize) { //if we've went out of scope of the buffer
				this._bufferReadPos += this._bufferPos; //re-read the buffer
				this._bufferPos = 0;

				this._stream.Seek(this._bufferReadPos, SeekOrigin.Begin);
				this._stream.Read(this._bufferRead, 0, BufferSize);
			}

			//return the position of bytes from the buffer

			this._bufferPos += amt;
			return this._bufferPos - amt;
		}

		private void BufferSeek(long pos) {
			if (Math.Abs(this._bufferReadPos - pos) >= BufferSize || pos <= this._bufferReadPos) {
				this._bufferReadPos = pos; //move the buffer reading pos
				this._bufferPos = 0; //move the buffer pos

				this._stream.Seek(pos, SeekOrigin.Begin);
				this._stream.Read(this._bufferRead, 0, BufferSize);
			} else this._bufferPos = (int)(pos - this._bufferReadPos);
		}
	}
}