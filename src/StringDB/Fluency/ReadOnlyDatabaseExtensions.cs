using JetBrains.Annotations;

using StringDB.Databases;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="ReadOnlyDatabase{TKey, TValue}"/>.
	/// </summary>
	[PublicAPI]
	public static class ReadOnlyDatabaseExtensions
	{
		/// <summary>
		/// Makes the database only able to be read.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="database">The database to make read only.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		/// <returns>A read only database</returns>
		public static IDatabase<TKey, TValue> AsReadOnly<TKey, TValue>
		(
			[NotNull] this IDatabase<TKey, TValue> database,
			bool disposeDatabase = true
		)
			=> new ReadOnlyDatabase<TKey, TValue>(database, disposeDatabase);
	}
}