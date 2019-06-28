namespace StringDB.Querying.Messaging
{
	/// <summary>
	/// Represents a message from a recepient.
	/// </summary>
	public struct Message<TMessage>
	{
		public static readonly Message<TMessage> DefaultLackingData = new Message<TMessage>
		{
			LacksData = true
		};

		/// <summary>
		/// The sender of the message.
		/// </summary>
		public IMessageClient<TMessage> Sender;

		/// <summary>
		/// The data of this message.
		/// </summary>
		public TMessage Data;

		/// <summary>
		/// If this message lacks any data.
		/// This is typically only true if
		/// a message client is getting disposed.
		/// </summary>
		public bool LacksData;
	}
}