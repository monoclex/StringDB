using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB {
/*
	//TODO:

	/// <summary>A wrapper around any object to make it thread safe.</summary>
	/// <typeparam name="T">The object to make thread safe.</typeparam>
	public class ThreadSafeWrapper<T> {

		/// <summary>Create a new ThreadSafeWrapper around the object.</summary>
		/// <param name="item">The item to make thread safe.</param>
		public ThreadSafeWrapper(T item) {
			this._lock = new object();
			this._object = item;
		}

		private object _lock { get; }
		private T _object { get; set; }

		/// <summary>The object that is now wrapped in thread-safe goodness.</summary>
		public T WrappedObject {
			get {
				lock(this._lock) {
					return this._object;
				}
			}
			set {
				lock(this._lock) {
					this._object = value;
				}
			}
		}
	}
*/
}
