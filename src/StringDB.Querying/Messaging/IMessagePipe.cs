using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	/// <summary>
	/// A pipe for intraprocess communication between different threads/tasks.
	/// </summary>
	[PublicAPI]
	public interface IMessagePipe<T> : IDisposable
	{
		/// <summary>
		/// Enqueues a message to the message pipe.
		/// </summary>
		/// <param name="message">The message to enqueue.</param>
		void Enqueue([NotNull] T message);

		/// <summary>
		/// Dequeues a message from the message pipe.
		/// Will not complete until there is something to dequeue.
		/// </summary>
		/// <param name="cancellationToken">The token used to cancel the operation</param>
		[NotNull, ItemNotNull]
		Task<T> Dequeue(CancellationToken cancellationToken = default);
	}
}