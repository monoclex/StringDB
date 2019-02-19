namespace StringDB.IO
{
	public sealed struct DatabaseItem
	{
		public byte[] Key;
		public long ValuePosition;
		public bool IsLast;
	}
}