//#define THREAD_SAFE

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace StringDB.Reader {
	public interface IRawReader {
		IPart ReadAt(long pos);
		IPart ReadOn(IPart previous);

		byte[] ReadDataValueAt(long p);
	}

	public class RawReader : IRawReader {
		public RawReader(Stream s, object @lock = null) {
			this._stream = s;
			this._br = new BinaryReader(s);
			this._lock = @lock;

			if (this._lock == null)
				this._lock = new object();
		}

		private Stream _stream;
		private BinaryReader _br;

		private object _lock = null;

		private long _lastPos = -1;

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

			var p = this.ReadBytes(9);

			if (this._bufferRead[p] == Consts.IndexSeperator) {
				var ic = BitConverter.ToInt64(this._bufferRead, p + 1);
				//TODO: remove if statement
				if (ic == 0)
					return null;
				else
					return new PartIndexChain(this._bufferRead[p], pos, ic);
			} else {
				var val_pos = this.ReadBytes(this._bufferRead[p]);

				byte[] val = new byte[this._bufferRead[p]];
				for (var i = 0; i < val.Length; i++)
					val[i] = this._bufferRead[val_pos + i];

				return new PartDataPair(this._bufferRead[p], pos, BitConverter.ToInt64(this._bufferRead, p + 1), val);
			}
#if THREAD_SAFE
			}
#endif
		}

		public IPart ReadOn(IPart previous) =>
			!(previous is IPartIndexChain) ?
				this.ReadAt(previous.NextPart)
				: !(previous.NextPart == 0) ?
					this.ReadAt(previous.NextPart)
					: null;

		public byte[] ReadDataValueAt(long p) {
#if THREAD_SAFE
			lock(_lock) {
#endif
			_Seek(p);

			byte[] b = this._br.ReadBytes(9);

			switch (b[0]) {
				case Consts.IsByteValue:
				this._stream.Seek(p + 1 + sizeof(byte), SeekOrigin.Begin);
				return this._br.ReadBytes(b[1]);

				case Consts.IsUShortValue:
				this._stream.Seek(p + 1 + sizeof(ushort), SeekOrigin.Begin);
				return this._br.ReadBytes(BitConverter.ToUInt16(b, 1));

				case Consts.IsUIntValue:
				this._stream.Seek(p + 1 + sizeof(uint), SeekOrigin.Begin);
				return this._br.ReadBytes((int)BitConverter.ToUInt32(b, 1));

				case Consts.IsULongValue:
				return this._br.ReadBytes((int)BitConverter.ToUInt64(b, 1));

				default:
				return null;
			}
#if THREAD_SAFE
			}
#endif
		}

		//heavily optimized method of reading bytes with an internal byte[] cache
		private int ReadBytes(int amt) {
			//we don't need this because we know we'll only be reading 9-253 bytes at a time ( or however many the index name is )
			/*
			bool situation = amt > BufferSize; //if we're reading more then the buffer size allows

			if (situation) throw new Exception("The amount of bytes we're reading is more then the buffer size."); //just read those

			//ok, now let's make sure that the buffer we have is updated
			*/

			//we don't need this because the +amt will do it for us
			if (/*this._bufferPos >= BufferSize ||*/ this._bufferPos + amt >= BufferSize) { //if we've went out of scope of the buffer
				this._bufferReadPos += this._bufferPos; //move the buffer reading pos
				this._bufferPos = 0; //move the buffer pos

				this._stream.Seek(this._bufferReadPos, SeekOrigin.Begin);
				this._stream.Read(this._bufferRead, 0, BufferSize);
			}

			//guarenteed to be false after the above code
			//situation = amt + _bufferPos > BufferSize;

			//if (!situation) { //if the amount of bytes and the position the buffer size is at is less then the buffer size
			//return the bytes from the buffer

			this._bufferPos += amt;
			return this._bufferPos - amt;
			/*} else {
				//the amount of bytes and the position within the buffer size is more then the buffer size, but the amount of bytes is less then the buffer size

				this._bufferReadPos += this._bufferPos; //set the new read position to the current pos in the buffer
				this._bufferPos = 0;
				this._stream.Seek(this._bufferReadPos, SeekOrigin.Begin);
				this._stream.Read(this._bufferRead, 0, BufferSize); //read at the new position into the buffer
				
				this._bufferPos += amt;
				return this._bufferPos - amt;
			}*/
		}

		private void _BufferSeek(long pos) {
			if (Math.Abs(this._bufferReadPos - pos) > BufferSize || pos < this._bufferReadPos) {
				this._bufferReadPos = pos; //move the buffer reading pos
				this._bufferPos = 0; //move the buffer pos

				this._stream.Seek(pos, SeekOrigin.Begin);
				this._stream.Read(this._bufferRead, 0, BufferSize);
			} else this._bufferPos = (int)(pos - this._bufferReadPos);
		}

		private void _Seek(long pos) {
			this._stream.Seek(pos, SeekOrigin.Begin);
		}
	}
}