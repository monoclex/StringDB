using JetBrains.Annotations;

using StringDB.Querying.Queries;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Extension methods that can be applied to an <see cref="IQueryManager{TKey, TValue}"/>.
	/// </summary>
	[PublicAPI]
	public static class QueryManagerExtensions
	{
		/// <summary>
		/// Finds the key value pair for a given key in a query manager.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="queryManager">The query manager to use.</param>
		/// <param name="isKey">The delegate that determines if the item
		/// is the one you are looking for.</param>
		/// <param name="cancellationToken">A cancellation token to request
		/// the cancellation of the query.</param>
		/// <returns>An awaitable task that will return your key
		/// and value.</returns>
		[NotNull, ItemCanBeNull]
		public static async Task<KeyValuePair<TKey, TValue>?> Find<TKey, TValue>
		(
			[NotNull] this IQueryManager<TKey, TValue> queryManager,
			[NotNull] Func<TKey, bool> isKey,
			[CanBeNull] CancellationToken cancellationToken = default
		)
		{
			var query = new FindQuery<TKey, TValue>(isKey, cancellationToken);

			if (!await queryManager.ExecuteQuery(query).ConfigureAwait(false))
			{
				return null;
			}

			return new KeyValuePair<TKey, TValue>(query.Key, query.Value);
		}
	}
}