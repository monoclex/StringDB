using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {
	internal partial interface IStreamIO {
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
			TypeHandlerLengthManager.WriteLength(this._bw, len);
			typeHandler.Write(this._bw, itm);
			return WriteValueSize(len);
		}

		public long WriteValueSize(long len)
			=> TypeHandlerLengthManager.EstimateWriteLengthSize(len) + len;
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