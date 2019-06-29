namespace StringDB.Querying.Messaging
{
	public static class MessageClientExtensions
	{
		/// <summary>
		/// Sends a message to a recipient, specified by the message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <returns>An awaitable task.</returns>
		public static void Send<TMessage>
		(
			this IMessageClient<TMessage> messageClient,
			IMessageClient<TMessage> target,
			TMessage message
		)
			=> target.Queue
			(
				new Message<TMessage>
				{
					Sender = messageClient,
					Data = message
				}
			);
	}
}