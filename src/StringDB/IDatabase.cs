using JetBrains.Annotations;

using System;
using System.Collections.Generic;

namespace StringDB
{
	/// <summary>
	/// A database.
	/// </summary>
	/// <typeparam name="TKey">The type of key of the database.</typeparam>
	/// <typeparam name="TValue">The type of value of the database.</typeparam>
	[PublicAPI]
	public interface IDatabase<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>>, IDisposable
	{
		/// <summary>
		/// Gets the first value with the specified key.
		/// If the value is unable to be found, an exception is thrown.
		/// </summary>
		/// <param name="key">The key to find the first value with.</param>
		/// <returns>The first value associated the key.</returns>
		[NotNull] TValue Get([NotNull] TKey key);

		/// <summary>
		/// Tries to get a value of the specified key.
		/// </summary>
		/// <param name="key">The key to use.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if a value was found, <c>false</c> if it was not.</returns>
		bool TryGet([NotNull] TKey key, [CanBeNull] out TValue value);

		/// <summary>
		/// Gets every lazy loading value based on a key
		/// </summary>
		/// <param name="key">The key to use</param>
		/// <returns>Every value associated with a key</returns>
		[NotNull] IEnumerable<ILazyLoader<TValue>> GetAll([NotNull] TKey key);

		/// <summary>
		/// Inserts a single element into the database.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void Insert([NotNull] TKey key, [NotNull] TValue value);

		/// <summary>
		/// Inserts a range of items into the database.
		/// </summary>
		/// <param name="items">The items to insert.</param>
		void InsertRange([NotNull] params KeyValuePair<TKey, TValue>[] items);
	}
}