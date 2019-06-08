using JetBrains.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Represents a query used for finding a value.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public class FindQuery<TKey, TValue> : IQuery<TKey, TValue>
	{
		private readonly Func<TKey, bool> _isItem;
		private readonly CancellationToken _cancellationToken;

		public FindQuery
		(
			Func<TKey, bool> isItem,
			CancellationToken cancellationToken = default
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

		public bool IsCancellationRequested => _cancellationToken.IsCancellationRequested;

		public async Task<QueryAcceptance> Accept([NotNull] TKey key, [NotNull] IRequest<TValue> value)
		{
			await Task.Yield();

			if (_isItem(key))
			{
				return QueryAcceptance.Completed;
			}

			return QueryAcceptance.NotAccepted;
		}

		public async Task Process([NotNull] TKey key, [NotNull] IRequest<TValue> value)
		{
			Key = key;
			Value = await value.Request().ConfigureAwait(false);
		}

		public void Dispose()
		{
		}
	}
}