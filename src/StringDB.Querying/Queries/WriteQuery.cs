using JetBrains.Annotations;

using System;

namespace StringDB.Querying.Queries
{
	/// <summary>
	/// A simple implementation of the <see cref="IWriteQuery{TKey, TValue}"/> interface.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	[PublicAPI]
	public class WriteQuery<TKey, TValue> : IWriteQuery<TKey, TValue>
	{
		private readonly Action<IDatabase<TKey, TValue>> _execute;

		/// <summary>
		/// Creates a new write query.
		/// </summary>
		/// <param name="execute">The method to execute.</param>
		public WriteQuery
		(
			[NotNull] Action<IDatabase<TKey, TValue>> execute
		)
			=> _execute = execute;

		public void Execute(IDatabase<TKey, TValue> database)
			=> _execute(database);

		public void Dispose()
		{
		}
	}
}