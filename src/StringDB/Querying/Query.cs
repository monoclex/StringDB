using JetBrains.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A simple implementation of the <see cref="IQuery{TKey, TValue}"/> interface.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	[PublicAPI]
	public class Query<TKey, TValue> : IQuery<TKey, TValue>
	{
		private readonly Func<TKey, TValue, Task<QueryAcceptance>> _accept;
		private readonly Func<TKey, TValue, Task> _process;
		private readonly CancellationToken _cancellationToken;

		/// <summary>
		/// Create a new query.
		/// </summary>
		/// <param name="accept">The delegate for accepting a query.</param>
		/// <param name="process">The delegate for processing a query.</param>
		public Query
		(
			Func<TKey, TValue, Task<QueryAcceptance>> accept,
			Func<TKey, TValue, Task> process,
			CancellationToken cancellationToken = default
		)
		{
			_accept = accept;
			_process = process;
			_cancellationToken = cancellationToken;
		}

		public bool IsCancelled => !_cancellationToken.IsCancellationRequested;

		public Task<QueryAcceptance> Accept(TKey key, TValue value)
			=> _accept(key, value);

		public Task Process(TKey key, TValue value)
			=> _process(key, value);

		public void Dispose()
		{
		}
	}
}