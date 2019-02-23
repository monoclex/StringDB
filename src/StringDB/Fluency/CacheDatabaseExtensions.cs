using StringDB.Databases;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="CacheDatabase{TKey,TValue}"/>.
	/// </summary>
	public static class CacheDatabaseExtensions
	{
		/// <summary>
		/// Apply a non-thread-safe cache to a database.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="database">The database to cache results from.</param>
		/// <returns>A database with caching.</returns>
		public static IDatabase<TKey, TValue> WithCache<TKey, TValue>(this IDatabase<TKey, TValue> database)
			=> new CacheDatabase<TKey, TValue>(database);
	}
}