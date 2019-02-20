namespace StringDB.IO
{
	public struct DatabaseItem
	{
		public byte[] Key;

		public long DataPosition;

		/// <summary>
		/// If this is true, there are no more values left to read and this item itself isn't a readable value.
		/// </summary>
		public bool EndOfItems;
	}
}