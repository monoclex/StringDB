using JetBrains.Annotations;

namespace StringDB.IO
{
	public struct LowLevelDatabaseItem
	{
		[NotNull]
		public byte[] Index;

		public long DataPosition;
	}
}