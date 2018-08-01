using StringDB.Exceptions;

using System;
using System.IO;

namespace StringDB.Reader {

	internal interface IRawReader {

		IPart ReadAt(long pos);

		IPart ReadOn(IPart previous);

		ITypeHandler ReadType(long pos, ITypeHandler typeHandlerReadWith, byte? specifyType = null);

		T ReadData<T>(long pos, ITypeHandler typeHandlerReadWith, long len = -1);

		T ReadDataAs<T>(long pos, ITypeHandler typeHandlerReadWith, long len = -1);

		long ReadLength(long pos);

		byte ReadByte(long pos);

		void DrainBuffer();
	}

	internal class ThreadSafeRawReader : IRawReader {

		public ThreadSafeRawReader(IRawReader parent, object @lock) {
			this._parent = parent;
			this._lock = @lock;
		}

		private readonly IRawReader _parent;
		private readonly object _lock;

		public IPart ReadAt(long pos) {
			lock (this._lock) return this._parent.ReadAt(pos);
		}

		public IPart ReadOn(IPart previous) {
			lock (this._lock) return this._parent.ReadOn(previous);
		}

		public ITypeHandler ReadType(long pos, ITypeHandler typeHandlerReadWith, byte? specifyType = null) {
			lock (this._lock) return this._parent.ReadType(pos, typeHandlerReadWith, specifyType);
		}

		public T ReadData<T>(long pos, ITypeHandler typeHandlerReadWith, long len = Consts.NOSPECIFYLEN) {
			lock (this._lock) return this._parent.ReadData<T>(pos, typeHandlerReadWith, len);
		}

		public T ReadDataAs<T>(long pos, ITypeHandler typeHandlerReadWith, long len = Consts.NOSPECIFYLEN) {
			lock (this._lock) return this._parent.ReadDataAs<T>(pos, typeHandlerReadWith, len);
		}

		public long ReadLength(long pos) {
			lock (this._lock) return this._parent.ReadLength(pos);
		}

		public byte ReadByte(long pos) {
			lock (this._lock) return this._parent.ReadByte(pos);
		}

		public void DrainBuffer() {
			lock (this._lock) this._parent.DrainBuffer();
		}
	}

	//TODO: cleanup this mESS

	internal class RawReader : IRawReader {

		internal RawReader(Stream s) {
			this._stream = s;
			this._br = new BinaryReader(s);
		}

		private readonly Stream _stream;
		private readonly BinaryReader _br;

		private const int BufferSize = 0x1000; // 4096KB buffer
		private const int MinusBufferSize = -1 - BufferSize;

		private long _bufferReadPos = MinusBufferSize; //the position we read the buffer at in the filestream
		private int _bufferPos = MinusBufferSize; //the position within the buffer
		private readonly byte[] _bufferRead = new byte[BufferSize];

		private readonly byte[] _oneByteBuffer = new byte[1];

		public IPart ReadAt(long pos) {
			BufferSeek(pos);

			var p = this.ReadBytes(10); // read the length, and the data pos
			var importantByte = this._bufferRead[p]; // store the index type incase the buffer updates
			var intVal = BitConverter.ToInt64(this._bufferRead, p + 1); // use BitConverter to get it as a long

			if (importantByte == Consts.IndexSeperator) { // if it's an index seperator
				return
					intVal == 0 ? // if it goes to 0, we know we've hit the end of the DB.
					(IPart)null
					: new PartIndexChain(importantByte, pos, intVal);
			} else {
				if (importantByte == Consts.NoIndex) return null; // if the index length is 0, we know we've probably hit the end of the DB as well.

				var byteType = this._bufferRead[p + 9];
				var val_pos = this.ReadBytes(importantByte); // read the length of the index
				var val = new byte[importantByte];

				for (var i = 0; i < val.Length; i++) // loop through the buffer and read bytes ( safe because buffer is larger then Consts.MaxLength )
					val[i] = this._bufferRead[val_pos + i];

				return new PartDataPair(importantByte, pos, intVal, val, byteType);
			}
		}

		public IPart ReadOn(IPart previous) => // only read on if the next part can be found
			previous.NextPart != 0 ?
				this.ReadAt(previous.NextPart)
				: null;

		public T ReadData<T>(long pos, ITypeHandler typeHandlerReadWith, long len = Consts.NOSPECIFYLEN) {
			var type = ReadType(pos, null, (byte?)null); // get the proper type handler

			// throw an exception if we're reading the wrong type
			if (type.Type != typeof(T)) throw new TypesDontMatch(typeof(T), type.Type);

			var typ = (typeHandlerReadWith as TypeHandler<T>);

			// read it properly

			return len == Consts.NOSPECIFYLEN ?
				typ.Read(this._br)
				: typ.Read(this._br, len);
		}

		public T ReadDataAs<T>(long pos, ITypeHandler typeHandlerReadWith, long len = Consts.NOSPECIFYLEN) {
			this._stream.Seek(pos); // seek to the data and ignore the type identifier
			this._stream.Read(this._oneByteBuffer, 0, 1);

			var typ = (typeHandlerReadWith as TypeHandler<T>);

			return len == Consts.NOSPECIFYLEN ?
				typ.Read(this._br)
				: typ.Read(this._br, len);
		}

		public ITypeHandler ReadType(long pos, ITypeHandler typeHandlerReadWith, byte? specifyType = null) {
			if (specifyType == null) {
				this._stream.Seek(pos); // seek to the position
				byte b;
				return (b = this._br.ReadByte()) == typeHandlerReadWith?.Id ?
					typeHandlerReadWith
					: TypeManager.GetHandlerFor(b); // get the handler for the type of data
			} else return TypeManager.GetHandlerFor((byte)specifyType);
		}

		public long ReadLength(long pos) {
			this._stream.Seek(pos); // seek to the data and ignore the type
			this._stream.Read(this._oneByteBuffer, 0, 1);
			var b = this._br.ReadByte();

			if (b == 0) {
				b = 0;
			}

			this._br.BaseStream.Seek(-1, SeekOrigin.Current);

			return TypeHandlerLengthManager.ReadLength(this._br); // read the length of the data
		}

		public byte ReadByte(long pos) {
			this._stream.Seek(pos);
			this._stream.Read(this._oneByteBuffer, 0, 1);
			return this._oneByteBuffer[0];
		}

		public void DrainBuffer() {
			this._bufferPos = MinusBufferSize;
			this._bufferReadPos = MinusBufferSize;
			//we don't clear the actual byte[] buffer because that'll be done when we try to read it
		}

		//heavily optimized method of reading bytes with an internal byte[] cache
		private int ReadBytes(int amt) {
			if (this._bufferPos + amt >= BufferSize) { //if we've went out of scope of the buffer
													   // apparently this code *never* gets triggered by the debugger?
													   // dunno why it exists but ok

				this._bufferReadPos += this._bufferPos; //re-read the buffer
				this._bufferPos = 0;

				this._stream.Seek(this._bufferReadPos, SeekOrigin.Begin);
				var len = this._stream.Read(this._bufferRead, 0, BufferSize);

				//TODO: explore possibility of exploit due to not tracking the size of the buffer
				// for now, we'll just clear out the rest of the bytes in the buffer starting from the length.
				// It'll work well *enough* as a fix.

				if (len != BufferSize)
					for (var i = len; i < BufferSize; i++)
						this._bufferRead[i] = 0x00;
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
				var len = this._stream.Read(this._bufferRead, 0, BufferSize);

				//TODO: explore possibility of exploit due to not tracking the size of the buffer
				// for now, we'll just clear out the rest of the bytes in the buffer starting from the length.
				// It'll work well *enough* as a fix.

				if (len != BufferSize)
					for (var i = len; i < BufferSize; i++)
						this._bufferRead[i] = 0x00;
			} else this._bufferPos = (int)(pos - this._bufferReadPos);
		}
	}
}