using System;

namespace StringDB.Reader {

	internal struct ThreadSafeRuntimeValue : IRuntimeValue {

		public ThreadSafeRuntimeValue(IRuntimeValue parent, object @lock) {
			this._parent = parent;
			this._lock = @lock;
		}

		private readonly IRuntimeValue _parent;
		private readonly object _lock;

		/// <inheritdoc/>
		public T Get<T>() {
			lock (this._lock) return this._parent.Get<T>();
		}

		/// <inheritdoc/>
		public T Get<T>(TypeHandler<T> typeHandler) {
			lock (this._lock) return this._parent.Get(typeHandler);
		}

		/// <inheritdoc/>
		public T GetAs<T>() {
			lock (this._lock) return this._parent.GetAs<T>();
		}

		/// <inheritdoc/>
		public T GetAs<T>(TypeHandler<T> typeHandler) {
			lock (this._lock) return this._parent.GetAs(typeHandler);
		}

		/// <inheritdoc/>
		public Type Type() {
			lock (this._lock) return this._parent.Type();
		}

		/// <inheritdoc/>
		public long Length() {
			lock (this._lock) return this._parent.Length();
		}
	}
}