using System;
using System.Collections.Generic;

namespace StringDB
{
	/// <summary>
	/// A database.
	/// </summary>
	/// <typeparam name="TKey">The type of key of the database.</typeparam>
	/// <typeparam name="TValue">The type of value of the database.</typeparam>
	public interface IDatabase<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>>, IDisposable
	{
		/// <summary>
		/// Gets the first value with the specified key.
		/// If the value is unable to be found, an exception is thrown.
		/// </summary>
		/// <param name="key">The key to find the first value with.</param>
		/// <returns>The first value associated the key.</returns>
		TValue Get(TKey key);

		/// <summary>
		/// Tries to get a value of the specified key.
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if a value was found, <c>false</c> if it was not.</returns>
		bool TryGet(TKey key, out TValue value);

		/// <summary>
		/// Gets every lazy loading value based on a key
		/// </summary>
		/// <param name="key">The key to use</param>
		/// <returns>Every value associated with a key</returns>
		IEnumerable<ILazyLoader<TValue>> GetAll(TKey key);

		/// <summary>
		/// Inserts a single element into the database.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void Insert(TKey key, TValue value);

		/// <summary>
		/// Inserts a range of items into the database.
		/// </summary>
		/// <param name="items">The items to insert.</param>
		void InsertRange(KeyValuePair<TKey, TValue>[] items);
	}
}