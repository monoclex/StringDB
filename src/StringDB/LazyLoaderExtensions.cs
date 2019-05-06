using JetBrains.Annotations;

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StringDB
{
	/// <summary>
	/// Extension methods on an <see cref="ILazyLoader{T}"/>.
	/// </summary>
	[PublicAPI]
	public static class LazyLoaderExtensions
	{
		/// <summary>
		/// Allows the usage of the await keyword for the <paramref name="lazyLoader"/>.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="lazyLoader">The lazy loader to get an awaiter for.</param>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LazyLoaderAwaiter<T> GetAwaiter<T>(this ILazyLoader<T> lazyLoader)
			=> new LazyLoaderAwaiter<T>
			{
				LazyLoader = lazyLoader
			};

		/// <summary>
		/// Turns any ILazyLoader into a KeyValuePair with the key
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="lazyLoader">The lazy loader.</param>
		/// <param name="key">The key.</param>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static KeyValuePair<TKey, ILazyLoader<TValue>> ToKeyValuePair<TKey, TValue>
		(
			this ILazyLoader<TValue> lazyLoader,
			TKey key
		)
			=> new KeyValuePair<TKey, ILazyLoader<TValue>>(key, lazyLoader);
	}
}