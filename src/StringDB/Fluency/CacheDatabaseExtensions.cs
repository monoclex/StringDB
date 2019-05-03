using JetBrains.Annotations;

using StringDB.Databases;

using System.Runtime.CompilerServices;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="CacheDatabase{TKey,TValue}"/>.
	/// </summary>
	[PublicAPI]
	public static class CacheDatabaseExtensions
	{
		/// <summary>
		/// Apply a non-thread-safe cache to a database.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="database">The database to cache results from.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		/// <returns>A database with caching.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TKey, TValue> WithCache<TKey, TValue>
		(
			[NotNull] this IDatabase<TKey, TValue> database,
			bool disposeDatabase = true
		)
			=> new CacheDatabase<TKey, TValue>(database, disposeDatabase);
	}
}