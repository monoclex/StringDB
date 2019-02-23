using System;

namespace StringDB.IO
{
	/// <inheritdoc />
	/// <summary>
	/// Used for StringDB based databases.
	/// </summary>
	public interface ILowlevelDatabaseIODevice : IDisposable
	{
		long JumpPos { get; set; }

		long GetPosition();

		void Reset();

		void SeekEnd();

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