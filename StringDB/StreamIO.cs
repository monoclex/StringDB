using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {

	 // Purpose: Take the majority of the work of the RawReader and RawWriter and put it into one class.
	 // Makes it easier to modify the file format later.

	internal partial interface IStreamIO : IDisposable {
		long WriteIndex<T>(TypeHandler<T> typeHandler, long itmLen, T itm, long dataPos);
		long WriteIndexSize(long itmLen);

		long WriteJump(long jmpTo);
		long WriteJumpSize();

		long WriteValue<T>(TypeHandler<T> typeHandler, long len, T itm);
		long WriteValueSize(long len);
	}

	internal class StreamIO : IStreamIO {
		public StreamIO(Stream stream) {
			this._stream = stream;
			this._bw = new BinaryWriter(this._stream);
			this._br = new BinaryReader(this._stream);
		}

		private Stream _stream;
		private BinaryWriter _bw;
		private BinaryReader _br;

		public Stream Stream => this._stream;
		public BinaryWriter BinaryWriter => this._bw;
		public BinaryReader BinaryReader => this._br;

		public long Length => this.Stream.Length;
		public long Position => this.Stream.Position;

		public void Seek(long pos)
			=> this._stream.Seek(pos, SeekOrigin.Begin);

		public void Flush()
			=> this._stream.Flush();

		public long WriteIndex<T>(TypeHandler<T> typeHandler, long itmLen, T itm, long dataPos) {
			var len = (byte)itmLen;

			this._bw.Write(len);
			this._bw.Write(dataPos);
			this._bw.Write(typeHandler.Id);
			typeHandler.Write(this._bw, itm);

			return WriteIndexSize(itmLen);
		}

		public long WriteIndexSize(long itmLen)
			=> sizeof(byte) + sizeof(long) + sizeof(byte) + itmLen;

		public long WriteJump(long jmpTo) {
			this._bw.Write(Consts.IndexSeperator);
			this._bw.Write(jmpTo);

			return WriteJumpSize();
		}

		public long WriteJumpSize()
			=> sizeof(byte) + sizeof(long);

		public long WriteValue<T>(TypeHandler<T> typeHandler, long len, T itm) {
			this._bw.Write(typeHandler.Id);
			TypeHandlerLengthManager.WriteLength(this._bw, len);
			typeHandler.Write(this._bw, itm);
			return WriteValueSize(len);
		}

		public long WriteValueSize(long len)
			=> TypeHandlerLengthManager.EstimateWriteLengthSize(len) + len;

		public IPart ReadAt(long pos) {
			this.Seek(pos);

			var _buffer = new byte[10];

			if (this._stream.Read(_buffer, 0, 10) < 10) // EOF
				return null;

			var importantByte = _buffer[0]; // get the length of the byte ( or a command )
			var intVal = BitConverter.ToInt64(_buffer, 1);

			if (importantByte == Consts.IndexSeperator) {
				// if the dataPos is 0, we've reached the end of the file.
				if (intVal == 0) return null;
				else return new PartIndexChain(importantByte, pos, intVal);
			} else {
				if (importantByte == Consts.NoIndex) return null; // if the index length is 0, we know we've probably hit the end of the DB as well.

				// read in the index name
				var byteType = _buffer[9];

				var val = new byte[importantByte];
				this._stream.Read(val, 0, importantByte);

				// new data pair
				return new PartDataPair(importantByte, pos, intVal, val, byteType);
			}
		}

		public void Dispose() {
			this._bw.Flush();
			this._stream.Flush();

			this._bw.Dispose();
			this._br.Dispose();
			this._stream.Dispose();
		}
	}

	//TODO: merge into StreamIO

	internal static class TypeHandlerLengthManager {

		public static void WriteLength(BinaryWriter bw, long len) { // write out the length
			if (len <= byte.MaxValue) { // depending on the length
				bw.Write(Consts.IsByteValue); // write the identifier for it
				bw.Write((byte)len); // and the length
			} else if (len <= ushort.MaxValue) {
				bw.Write(Consts.IsUShortValue);
				bw.Write((ushort)len);
			} else if (len <= uint.MaxValue) {
				bw.Write(Consts.IsUIntValue);
				bw.Write((uint)len);
			} else {
				bw.Write(Consts.IsLongValue);
				bw.Write(len);
			}
		}

		public static long ReadLength(BinaryReader br) { // read the length depending on the identifier
			long p;
			byte b;
			switch ((b = br.ReadByte())) {
				case Consts.IsByteValue:
				p = br.ReadByte();
				break;

				case Consts.IsUShortValue:
				p = br.ReadUInt16();
				break;

				case Consts.IsUIntValue:
				p = br.ReadUInt32();
				break;

				case Consts.IsLongValue:
				p = br.ReadInt64();
				break;

				default: return 0;
			}

			if (p < 1) {
				p = -1;
			}

			return p;
		}

		internal static long EstimateWriteLengthSize(long len) { // estimate how big in bytes a WriteLength would be with a given length
			const int length = sizeof(byte) + sizeof(byte);

			return len <= byte.MaxValue ?
				length + sizeof(byte)
				: len <= ushort.MaxValue ?
					length + sizeof(ushort)
					: len <= uint.MaxValue ?
						length + sizeof(uint)
						: length + sizeof(long);
		}
	}
}