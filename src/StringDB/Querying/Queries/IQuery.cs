using JetBrains.Annotations;

using System;
using System.Threading.Tasks;

namespace StringDB.Querying.Queries
{
	/// <summary>
	/// A query for an <see cref="IDatabase{TKey, TValue}"/>.
	/// Specifies what counts as a result, and provides a way to
	/// access the results of a query. Queries should be designed
	/// to be totally asynchronous, and should have no dependencies
	/// upon the order of items in a database, and should not expect
	/// any kind of logical firing order of events.
	/// </summary>
	[PublicAPI]
	public interface IQuery<in TKey, TValue> : IDisposable
	{
		/// <summary>
		/// If a cancellation is requested.
		/// </summary>
		bool IsCancellationRequested { get; }

		// TODO: ValueTask in net core 3
		/// <summary>
		/// Determines whether or not this query accepts a given result.
		/// This should only do the bare minimum amount of work to verify
		/// if a result qualifies, as there is a separate stage dedicated
		/// to processing results.
		/// </summary>
		/// <param name="key">The key of the entry in the database.</param>
		/// <param name="value">A request to get the value of the entry in the database.</param>
		/// <returns>True if the item is to be accepted, false if it is not.</returns>
		[NotNull]
		Task<QueryAcceptance> Process([NotNull] TKey key, [NotNull] IRequest<TValue> value);
	}
}