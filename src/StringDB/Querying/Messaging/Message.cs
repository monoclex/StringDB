namespace StringDB.Querying.Messaging
{
	/// <summary>
	/// Represents a message from a recepient.
	/// </summary>
	public struct Message<TMessage>
	{
		/// <summary>
		/// The sender of the message.
		/// </summary>
		public IMessageClient<TMessage> Sender;

		/// <summary>
		/// The data of this message.
		/// </summary>
		public TMessage Data;
	}
}