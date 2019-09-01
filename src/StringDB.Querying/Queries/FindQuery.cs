using JetBrains.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Queries
{
	/// <summary>
	/// Represents a query used for finding a value.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	[PublicAPI]
	public class FindQuery<TKey, TValue> : IQuery<TKey, TValue>
	{
		private readonly Func<TKey, bool> _isItem;
		private readonly CancellationTokenSource _cts;

		public FindQuery
		(
			[NotNull] Func<TKey, bool> isItem,
			[CanBeNull] CancellationToken cancellationToken = default
		)
		{
			_isItem = isItem;
			_cts = new CancellationTokenSource();
			CancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken).Token;
		}

		/// <summary>
		/// The key as a result of processing.
		/// </summary>
		public TKey Key { get; private set; }

		/// <summary>
		/// The value as a result of processing.
		/// </summary>
		public TValue Value { get; private set; }

		public CancellationToken CancellationToken { get; }

		public async Task<QueryAcceptance> Process([NotNull] TKey key, [NotNull] IRequest<TValue> value)
		{
			if (!_isItem(key))
			{
				return QueryAcceptance.Continue;
			}

			Key = key;
			Value = await value.Request()
				.ConfigureAwait(false);

			return QueryAcceptance.Completed;
		}

		public void Dispose() => _cts.Cancel();
	}
}