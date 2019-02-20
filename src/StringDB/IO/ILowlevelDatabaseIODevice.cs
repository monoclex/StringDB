using System;

namespace StringDB.IO
{
	public enum NextItemPeek
	{
		Index,
		Jump,
		EOF
	}

	public struct LowLevelDatabaseItem
	{
		public byte[] Index;
		public long DataPosition;
	}

	public interface ILowlevelDatabaseIODevice : IDisposable
	{
		long GetPosition();

		void Reset();

		void Seek(long position);

		void Flush();

		NextItemPeek Peek();

		LowLevelDatabaseItem ReadIndex();

		byte[] ReadValue(long dataPosition);

		long ReadJump();

		void WriteJump(long jumpTo);

		void WriteIndex(byte[] key, long dataPosition);

		void WriteValue(byte[] value);

		int CalculateIndexOffset(byte[] key);

		int CalculateValueOffset(byte[] value);

		int JumpOffsetSize { get; }
	}
}