using JetBrains.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Queries
{
	/// <summary>
	/// A simple implementation of the <see cref="IQuery{TKey, TValue}"/> interface.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	[PublicAPI]
	public class Query<TKey, TValue> : IQuery<TKey, TValue>
	{
		private readonly Func<TKey, IRequest<TValue>, Task<QueryAcceptance>> _process;
		private readonly CancellationToken _cancellationToken;
		private bool _disposed;

		/// <summary>
		/// Create a new query.
		/// </summary>
		/// <param name="process">The delegate for processing a query.</param>
		public Query
		(
			[NotNull] Func<TKey, IRequest<TValue>, Task<QueryAcceptance>> process,
			[CanBeNull] CancellationToken cancellationToken = default
		)
		{
			_process = process;
			_cancellationToken = cancellationToken;
		}

		public bool IsCancellationRequested => _cancellationToken.IsCancellationRequested || _disposed;

		public Task<QueryAcceptance> Process(TKey key, IRequest<TValue> value)
			=> _process(key, value);

		public void Dispose() => _disposed = true;
	}
}