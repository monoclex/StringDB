namespace StringDB.IO
{
	public struct DatabaseItem
	{
		public byte[] Key;
		public long ValuePosition;
		public bool IsLast;
	}
}