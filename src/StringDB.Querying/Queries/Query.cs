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
		private readonly CancellationTokenSource _cts;

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
			_cts = new CancellationTokenSource();

			CancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken).Token;
		}

		public CancellationToken CancellationToken { get; }

		public Task<QueryAcceptance> Process(TKey key, IRequest<TValue> value)
			=> _process(key, value);

		public void Dispose() => _cts.Cancel();
	}
}