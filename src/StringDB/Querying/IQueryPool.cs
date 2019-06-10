using JetBrains.Annotations;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A pool for queries. Handles killing or finishing
	/// queries when they are done, and allows for execution
	/// of all queries immediately.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	[PublicAPI]
	public interface IQueryPool<TKey, TValue>
	{
		/// <summary>
		/// Gets a read-only collection of the queries currently
		/// in the pool.
		/// </summary>
		/// <returns>The current queries in the pool/</returns>
		IReadOnlyCollection<QueryItem<TKey, TValue>> CurrentQueries();

		/// <summary>
		/// Appends a query to the pool.
		/// </summary>
		/// <param name="queryItem">The query item to append.</param>
		/// <returns>An awaitable <see cref="Task"/>.</returns>
		Task Append(QueryItem<TKey, TValue> queryItem);

		/// <summary>
		/// Executes every query asynchronously.
		/// </summary>
		/// <param name="index">The index of the database item.</param>
		/// <param name="key">The key to execute them all with.</param>
		/// <param name="value">The value to execute them all with.</param>
		/// <returns>An awaitable <see cref="Task"/>.</returns>
		Task ExecuteQueries(int index, TKey key, IRequest<TValue> value);
	}
}