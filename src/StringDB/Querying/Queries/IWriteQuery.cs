using JetBrains.Annotations;

using System;

namespace StringDB.Querying.Queries
{
	/// <summary>
	/// A query that modifies the database in some way.
	/// </summary>
	[PublicAPI]
	public interface IWriteQuery<TKey, TValue> : IDisposable
	{
		/// <summary>
		/// Allows operations to be performed on a database, such as
		/// those that require writing.
		/// </summary>
		/// <param name="database">The database to modify.</param>
		void Execute([NotNull] IDatabase<TKey, TValue> database);
	}
}