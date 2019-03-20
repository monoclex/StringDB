using JetBrains.Annotations;

using System;

namespace StringDB.IO
{
	/// <inheritdoc />
	/// <summary>
	/// Used for StringDB based databases.
	/// </summary>
	[PublicAPI]
	public interface ILowlevelDatabaseIODevice : IDisposable
	{
		long JumpPos { get; set; }

		long GetPosition();

		void Reset();

		void SeekEnd();

		void Seek(long position);

		void Flush();

		// peekResult is tightly coupled with ReadIndex

		NextItemPeek Peek(out byte peekResult);

		LowLevelDatabaseItem ReadIndex(byte peekResult);

		[NotNull]
		byte[] ReadValue(long dataPosition);

		long ReadJump();

		void WriteJump(long jumpTo);

		void WriteIndex([NotNull] byte[] key, long dataPosition);

		void WriteValue([NotNull] byte[] value);

		int CalculateIndexOffset([NotNull] byte[] key);

		int CalculateValueOffset([NotNull] byte[] value);

		int JumpOffsetSize { get; }
	}
}