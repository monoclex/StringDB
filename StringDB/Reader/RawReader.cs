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

		internal RawReader(StreamIO s) {
			this._sio = s;
		}

		private readonly StreamIO _sio;

		private byte[] _oneByteBuffer = new byte[1];

		public IPart ReadAt(long pos)
			=> this._sio.ReadAt(pos);

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
				typ.Read(this._sio.BinaryReader)
				: typ.Read(this._sio.BinaryReader, len);
		}

		public T ReadDataAs<T>(long pos, ITypeHandler typeHandlerReadWith, long len = Consts.NOSPECIFYLEN) {
			this._sio.Seek(pos + 1); // seek to the data and ignore the type identifier

			var typ = (typeHandlerReadWith as TypeHandler<T>);

			return len == Consts.NOSPECIFYLEN ?
				typ.Read(this._sio.BinaryReader)
				: typ.Read(this._sio.BinaryReader, len);
		}

		public ITypeHandler ReadType(long pos, ITypeHandler typeHandlerReadWith, byte? specifyType = null) {
			if (specifyType == null) {
				this._sio.Seek(pos); // seek to the position
				byte b;
				return (b = this._sio.BinaryReader.ReadByte()) == typeHandlerReadWith?.Id ?
					typeHandlerReadWith
					: TypeManager.GetHandlerFor(b); // get the handler for the type of data
			} else return TypeManager.GetHandlerFor((byte)specifyType);
		}

		public long ReadLength(long pos) {
			this._sio.Seek(pos + 1);

			return TypeHandlerLengthManager.ReadLength(this._sio.BinaryReader); // read the length of the data
		}

		public void DrainBuffer() {

		}

		public byte ReadByte(long pos) {
			this._sio.Seek(pos);
			this._sio.Stream.Read(this._oneByteBuffer, 0, 1);
			return this._oneByteBuffer[0];
		}
	}
}