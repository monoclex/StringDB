using JetBrains.Annotations;

using System;

namespace StringDB.Querying
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
		void Execute(IDatabase<TKey, TValue> database);
	}
}