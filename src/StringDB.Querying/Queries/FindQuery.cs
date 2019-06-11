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
	public class FindQuery<TKey, TValue> : IQuery<TKey, TValue>
	{
		private readonly Func<TKey, bool> _isItem;
		private readonly CancellationToken _cancellationToken;

		public FindQuery
		(
			[NotNull] Func<TKey, bool> isItem,
			[CanBeNull] CancellationToken cancellationToken = default
		)
		{
			_isItem = isItem;
			_cancellationToken = cancellationToken;
		}

		private static Task<QueryAcceptance> _completed = Task.FromResult(QueryAcceptance.Completed);
		private static Task<QueryAcceptance> _notAccepted = Task.FromResult(QueryAcceptance.NotAccepted);

		/// <summary>
		/// The key as a result of processing.
		/// </summary>
		public TKey Key { get; private set; }

		/// <summary>
		/// The value as a result of processing.
		/// </summary>
		public TValue Value { get; private set; }

		public bool IsCancellationRequested => _cancellationToken.IsCancellationRequested;

		public Task<QueryAcceptance> Accept([NotNull] TKey key, [NotNull] IRequest<TValue> value)
			=> _isItem(key)
			? _completed
			: _notAccepted;

		public async Task Process([NotNull] TKey key, [NotNull] IRequest<TValue> value)
		{
			Key = key;
			Value = await value.Request()
				.ConfigureAwait(false);
		}

		public void Dispose()
		{
		}
	}
}