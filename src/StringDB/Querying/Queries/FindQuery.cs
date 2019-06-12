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
		private readonly CancellationToken _cancellationToken;
		private bool _disposed;

		public FindQuery
		(
			[NotNull] Func<TKey, bool> isItem,
			[CanBeNull] CancellationToken cancellationToken = default
		)
		{
			_isItem = isItem;
			_cancellationToken = cancellationToken;
		}

		/// <summary>
		/// The key as a result of processing.
		/// </summary>
		public TKey Key { get; private set; }

		/// <summary>
		/// The value as a result of processing.
		/// </summary>
		public TValue Value { get; private set; }

		public bool IsCancellationRequested => _cancellationToken.IsCancellationRequested || _disposed;

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

		public void Dispose() => _disposed = true;
	}
}