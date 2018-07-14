using System;

namespace StringDB.Reader {

	public interface IRuntimeValue {

		T Get<T>();

		T Get<T>(TypeHandler<T> typeHandler);

		T GetAs<T>();

		T GetAs<T>(TypeHandler<T> typeHandler);

		Type Type();
	}

	//TODO: wrap runtime value in thread safeness

	public struct RuntimeValue : IRuntimeValue {
		internal const int NOSPECIFYLEN = -1;

		//TODO: remove specifyType

		internal RuntimeValue(IRawReader rawReader, long readPos, byte? specifyType, long specifyLen = NOSPECIFYLEN) {
			this._specifyType = specifyType;
			this._specifyLen = specifyLen;
			this._rawReader = rawReader;
			this._readPos = readPos;
		}

		private IRawReader _rawReader;
		private long _specifyLen;
		private byte? _specifyType;
		internal long _readPos;

		//TODO: documentation

		public T Get<T>()
			=> this.Get<T>(TypeManager.GetHandlerFor<T>());

		public T GetAs<T>()
			=> this.GetAs<T>(TypeManager.GetHandlerFor<T>());

		public T Get<T>(TypeHandler<T> typeHandler)
			=> this._specifyLen == NOSPECIFYLEN ?
					this._rawReader.ReadData<T>(this._readPos, typeHandler)
					: this._rawReader.ReadData<T>(this._readPos, this._specifyLen, typeHandler);

		public T GetAs<T>(TypeHandler<T> typeHandler)
			=> this._specifyLen == NOSPECIFYLEN ?
					this._rawReader.ReadDataAs<T>(this._readPos, typeHandler)
					: this._rawReader.ReadDataAs<T>(this._readPos, this._specifyLen, typeHandler);

		public new Type Type()
			=> this._specifyType != null ?
					throw new Exception($"Cannot fetch type of an index")
					: this._rawReader.ReadType(this._readPos, null).Type;
	}
}