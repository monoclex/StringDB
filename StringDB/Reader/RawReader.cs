//#define THREAD_SAFE

using StringDB.DBTypes;
using System;
using System.IO;

namespace StringDB.Reader {

	internal interface IRawReader {

		T ReadData<T>(long pos);

		IPart ReadAt(long pos);

		IPart ReadOn(IPart previous);

		Stream GetStreamOfDataAt(long p);

		byte[] ReadDataValueAt(long p);

		long ReadDataValueLengthAt(long p);

		/// <summary>Clears out the buffer. Will cause performance issues if you do it too often.</summary>
		void DrainBuffer();
	}

	internal class RawReader : IRawReader {

		internal RawReader(Stream s, object @lock = null) {
			this._stream = s;
			this._br = new BinaryReader(s);
			this._lock = @lock;

			if (this._lock == null)
				this._lock = new object();
		}

		private Stream _stream;
		private BinaryReader _br;

		private object _lock = null;

		private static readonly int BufferSize = 0x1000; //1MiB
		private static readonly int MinusBufferSize = -1 - BufferSize;

		private long _bufferReadPos = MinusBufferSize; //the position we read the buffer at in the filestream
		private int _bufferPos = MinusBufferSize; //the position within the buffer
		private byte[] _bufferRead = new byte[BufferSize];

		public IPart ReadAt(long pos) {
#if THREAD_SAFE
			lock (_lock) {
#endif
			_BufferSeek(pos);

			var p = this.ReadBytes(9); //set the important values right NOW, since later the buffer can chnage and screw things up.
			var importantByte = this._bufferRead[p]; //set these variables incase the buffer changes later when reading more bytes
			var intVal = BitConverter.ToInt64(this._bufferRead, p + 1);

			if (importantByte == Consts.IndexSeperator) {
				if (intVal == 0)
					return null;
				else
					return new PartIndexChain(importantByte, pos, intVal);
			} else {
				if (importantByte == Consts.NoIndex) return null;

				var val_pos = this.ReadBytes(importantByte);

				var val = new byte[importantByte];
				for (var i = 0; i < val.Length; i++)
					val[i] = this._bufferRead[val_pos + i];

				return new PartDataPair(importantByte, pos, intVal, val);
			}
#if THREAD_SAFE
			}
#endif
		}

		public IPart ReadOn(IPart previous) =>
			!(previous is PartIndexChain) ?
				this.ReadAt(previous.NextPart)
				: !(previous.NextPart == 0) ?
					this.ReadAt(previous.NextPart)
					: null;

		public T ReadData<T>(long pos) {
			var type = ReadType(pos);
			if (type.Type != typeof(T)) throw new Exception($"The data you are trying to read is not of type {typeof(T)}, it is of type {type.Type}");
			return (type as TypeHandler<T>).Read(this._br);
		}

		public T ReadDataAs<T>(long pos) {
			this._stream.Seek(pos, SeekOrigin.Begin);
			this._br.ReadByte();
			var type = TypeManager.GetHandlerFor<T>();
			if (type.Type != typeof(T)) throw new Exception($"The data you are trying to read is not of type {typeof(T)}, it is of type {type.Type}");
			return (type as TypeHandler<T>).Read(this._br);
		}

		public ITypeHandler ReadType(long pos) {
			this._stream.Seek(pos, SeekOrigin.Begin);
			return TypeManager.GetHandlerFor(this._br.ReadByte());
		}

		public long ReadDataValueLengthAt(long p) {
#if THREAD_SAFE
			lock(_lock) {
#endif
			_Seek(p);

			var b = this._br.ReadBytes(9);

			switch (b[0]) {
				case Consts.IsByteValue:
				return b[1];

				case Consts.IsUShortValue:
				return BitConverter.ToUInt16(b, 1);

				case Consts.IsUIntValue:
				return BitConverter.ToUInt32(b, 1);

				case Consts.IsLongValue:
				return (long)BitConverter.ToUInt64(b, 1);
			}

			return 0;
#if THREAD_SAFE
			}
#endif
		}

		public Stream GetStreamOfDataAt(long p) {
#if THREAD_SAFE
			lock(_lock) {
#endif
			var len = ReadDataValueLengthAt(p);

			var newp = p + 1;

			if (len < byte.MaxValue)
				newp += sizeof(byte);
			else if (len < ushort.MaxValue)
				newp += sizeof(ushort);
			else if (len < uint.MaxValue)
				newp += sizeof(uint);
			else if ((ulong)len < ulong.MaxValue)
				newp += sizeof(ulong);


			return new StreamFragment(this._stream, newp, len);
#if THREAD_SAFE
			}
#endif
		}

		public byte[] ReadDataValueAt(long p) {
#if THREAD_SAFE
			lock(_lock) {
#endif
			_Seek(p); //don't use the readlength because it slows down performance

			var b = this._br.ReadBytes(9);

			switch (b[0]) {
				case Consts.IsByteValue:
				this._stream.Seek(p + 1 + sizeof(byte), SeekOrigin.Begin); //seek backwards a little in the stream to where the data stars
				return this._br.ReadBytes(b[1]); //read out that data to a byte array

				case Consts.IsUShortValue:
				this._stream.Seek(p + 1 + sizeof(ushort), SeekOrigin.Begin);
				return this._br.ReadBytes(BitConverter.ToUInt16(b, 1));

				case Consts.IsUIntValue:
				this._stream.Seek(p + 1 + sizeof(uint), SeekOrigin.Begin);
				return this._br.ReadBytes((int)BitConverter.ToUInt32(b, 1));

				case Consts.IsLongValue:
				return this._br.ReadBytes((int)BitConverter.ToUInt64(b, 1));
			}

			return null;
#if THREAD_SAFE
			}
#endif
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

		private void _BufferSeek(long pos) {
			if (Math.Abs(this._bufferReadPos - pos) >= BufferSize || pos <= this._bufferReadPos) {
				this._bufferReadPos = pos; //move the buffer reading pos
				this._bufferPos = 0; //move the buffer pos

				this._stream.Seek(pos, SeekOrigin.Begin);
				this._stream.Read(this._bufferRead, 0, BufferSize);
			} else this._bufferPos = (int)(pos - this._bufferReadPos);
		}

		private void _Seek(long pos) => this._stream.Seek(pos, SeekOrigin.Begin);
	}
}