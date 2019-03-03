using JetBrains.Annotations;

using System.Collections.Generic;

namespace StringDB.IO
{
	/// <inheritdoc />
	/// <summary>
	/// An <see cref="T:StringDB.IO.IDatabaseIODevice" /> that interfaces with <see cref="T:StringDB.IO.ILowlevelDatabaseIODevice" />
	/// </summary>
	[PublicAPI]
	public sealed class DatabaseIODevice : IDatabaseIODevice
	{
		public ILowlevelDatabaseIODevice LowLevelDatabaseIODevice { get; }

		public DatabaseIODevice
		(
			[NotNull] ILowlevelDatabaseIODevice lowlevelDBIOD
		)
			=> LowLevelDatabaseIODevice = lowlevelDBIOD;

		public void Reset() => LowLevelDatabaseIODevice.Reset();

		/// <inheritdoc />
		public byte[] ReadValue(long position)
		{
			// temporarily go to the position to read the value,
			// then seek back to the cursor position for reading
			var curPos = LowLevelDatabaseIODevice.GetPosition();

			var value = LowLevelDatabaseIODevice.ReadValue(position);

			LowLevelDatabaseIODevice.Seek(curPos);

			return value;
		}

		/// <inheritdoc />
		public DatabaseItem ReadNext()
		{
			// handle EOFs/Jumps
			var peek = LowLevelDatabaseIODevice.Peek();

			ExecuteJumps(ref peek);

			if (peek == NextItemPeek.EOF)
			{
				return new DatabaseItem
				{
					EndOfItems = true
				};
			}

			// peek HAS to be an Index at this point

			var item = LowLevelDatabaseIODevice.ReadIndex();

			return new DatabaseItem
			{
				Key = item.Index,
				DataPosition = item.DataPosition,
				EndOfItems = false
			};
		}

		private void ExecuteJumps(ref NextItemPeek peek)
		{
			while (peek == NextItemPeek.Jump)
			{
				var jump = LowLevelDatabaseIODevice.ReadJump();
				LowLevelDatabaseIODevice.Seek(jump);
				peek = LowLevelDatabaseIODevice.Peek();
			}
		}

		/// <inheritdoc />
		public void Insert(KeyValuePair<byte[], byte[]>[] items)
		{
			LowLevelDatabaseIODevice.SeekEnd();

			var offset = LowLevelDatabaseIODevice.GetPosition();

			UpdatePreviousJump(offset);

			// we need to calculate the total offset of all the indexes
			// then we write every index & increment the offset by the offset of each value
			// and then we write the values

			// phase 1: calculating total offset

			foreach (var kvp in items)
			{
				offset += LowLevelDatabaseIODevice.CalculateIndexOffset(kvp.Key);
			}

			// the jump offset is important, we will be jumping after
			offset += LowLevelDatabaseIODevice.JumpOffsetSize;

			// phase 2: writing each key
			//			and incrementing the offset by the value

			foreach (var kvp in items)
			{
				LowLevelDatabaseIODevice.WriteIndex(kvp.Key, offset);

				offset += LowLevelDatabaseIODevice.CalculateValueOffset(kvp.Value);
			}

			WriteJump();

			// phase 3: writing each value sequentially

			foreach (var kvp in items)
			{
				LowLevelDatabaseIODevice.WriteValue(kvp.Value);
			}
		}

		private void UpdatePreviousJump(long jumpTo)
		{
			var currentPosition = LowLevelDatabaseIODevice.GetPosition();

			if (LowLevelDatabaseIODevice.JumpPos != 0)
			{
				// goto old jump pos and overwrite it with the current jump pos
				LowLevelDatabaseIODevice.Seek(LowLevelDatabaseIODevice.JumpPos);
				LowLevelDatabaseIODevice.WriteJump(jumpTo);
			}

			LowLevelDatabaseIODevice.Seek(currentPosition);
		}

		private void WriteJump()
		{
			var position = LowLevelDatabaseIODevice.GetPosition();

			LowLevelDatabaseIODevice.JumpPos = position;
			LowLevelDatabaseIODevice.WriteJump(0);
		}

		public void Dispose()
		{
			LowLevelDatabaseIODevice.Flush();
			LowLevelDatabaseIODevice.Dispose();
		}
	}
}