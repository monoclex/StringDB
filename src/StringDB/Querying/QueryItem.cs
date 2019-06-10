using StringDB.Querying.Queries;

using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Used by an <see cref="IQueryPool{TKey, TValue}"/>
	/// to manage the query.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public struct QueryItem<TKey, TValue>
	{
		/// <summary>
		/// The completion source for the query. Setting this
		/// should directly affect the completion state of the
		/// query, from <see cref="IQueryManager{TKey, TValue}.ExecuteQuery(IQuery{TKey, TValue})"/>.
		/// </summary>
		public TaskCompletionSource<bool> CompletionSource;

		/// <summary>
		/// The query this query item focuses on.
		/// </summary>
		public IQuery<TKey, TValue> Query;

		/// <summary>
		/// The index this query item was appended to
		/// during a database search.
		/// </summary>
		public int Index;
	}
}