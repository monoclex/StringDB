using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	/// <summary>
	/// Represents a message client that can send
	/// and receieve messages from recipients.
	/// </summary>
	public interface IMessageClient<TMessage> : IDisposable
	{
		/// <summary>
		/// Receives a message from a sender.
		/// </summary>
		/// <returns>An awaitable task that will return the message.</returns>
		Task<Message<TMessage>> Receive(CancellationToken cancellationToken);

		/// <summary>
		/// The incoming place for messages to arrive - this will
		/// be called by a sender and is expected to go into some
		/// kind of queue for the receiver to handle.
		/// </summary>
		/// <param name="message">The message to go into a queue.</param>
		/// <returns>An awaitable task that will complete once it is queues.</returns>
		void Queue(Message<TMessage> message);
	}
}