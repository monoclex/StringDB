using JetBrains.Annotations;

using StringDB.Querying.Queries;

using System;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	// TODO: async disposable in net core 3
	/// <summary>
	/// Manages queries for a database. It provides a
	/// truly asynchronous interface over a synchronous
	/// database, allowing true concurrency. This should
	/// act as an all encompassing gateway manager for
	/// all requests to and from the database.
	/// </summary>
	[PublicAPI]
	public interface IQueryManager<TKey, TValue> : IDisposable
	{
		/// <summary>
		/// Executes a query. The query should be scheduled for
		/// completion in the future, and a task is returned which
		/// will complete once the query finishes.
		/// </summary>
		/// <param name="query">The query to queue up.</param>
		/// <returns>An awaitable <see cref="Task"/>.</returns>
		[NotNull]
		Task<bool> ExecuteQuery([NotNull] IQuery<TKey, TValue> query);

		/// <summary>
		/// Executes a write query. The query should be scheduled
		/// for completion in the future, and a task is returned
		/// which will complete once the query finishes.
		/// </summary>
		/// <param name="writeQuery">The write query to queue up.</param>
		/// <returns>An awaitable <see cref="Task"/>.</returns>
		[NotNull]
		Task ExecuteQuery([NotNull] IWriteQuery<TKey, TValue> writeQuery);
	}
}