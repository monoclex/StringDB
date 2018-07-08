﻿using System.IO;

namespace StringDB.DBTypes {
	public abstract class TypeHandler<T> {
		public TypeHandler() { }

		public abstract byte Id { get; }

		public abstract long GetLength(T item);
		public abstract void Write(BinaryWriter bw, T item);
		public abstract T Read(BinaryReader br);

		public void WriteLength(BinaryWriter bw, long len) {
			if(len <= byte.MaxValue) {
				bw.Write(Consts.IsByteValue);
				bw.Write((byte)len);
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

		public long ReadLength(BinaryReader br) {
			switch(br.ReadByte()) {
				case Consts.IsByteValue:
				return br.ReadByte();

				case Consts.IsUShortValue:
				return br.ReadUInt16();

				case Consts.IsUIntValue:
				return br.ReadUInt32();

				case Consts.IsLongValue:
				return br.ReadInt64();
			}

			return 0; // ? ? ?
		}
	}
}