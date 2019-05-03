using JetBrains.Annotations;

using StringDB.Databases;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="BufferedDatabase{TKey, TValue}"/>.
	/// </summary>
	[PublicAPI]
	public static class BufferedDatabaseExtensions
	{
		/// <summary>
		/// Apply a buffer to a database to ease the cost of multiple single inserts.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="database">The database to buffer.</param>
		/// <param name="bufferSize">The size of the buffer.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		/// <returns>A buffered database.</returns>
		public static IDatabase<TKey, TValue> WithBuffer<TKey, TValue>
		(
			[NotNull] this IDatabase<TKey, TValue> database,
			int bufferSize = 0x1000,
			bool disposeDatabase = true
		)
			=> new BufferedDatabase<TKey, TValue>(database, bufferSize, disposeDatabase);
	}
}