using JetBrains.Annotations;

using StringDB.Querying.Messaging;

using System;
using System.Collections.Generic;
using System.Threading;

namespace StringDB.Querying
{
	/// <summary>
	/// Manages the iteration over an <see cref="IDatabase{TKey, TValue}"/>.
	/// Acts as a gateway between producing items and the consumption of them.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[PublicAPI]
	public interface IIterationManager<TKey, TValue> : IDisposable
	{
		/// <summary>
		/// Iterates over the entire database, and puts result in the target.
		/// </summary>
		/// <param name="target">The pipe to use when giving results.</param>
		/// <param name="cancellationToken">The cancellation token to use to
		/// stop iteration.</param>
		[NotNull]
		IIterationHandle IterateTo
		(
			[NotNull] IMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> target,
			CancellationToken cancellationToken = default
		);
	}
}