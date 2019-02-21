using System.Collections.Generic;

namespace StringDB.IO
{
	/// <inheritdoc />
	/// <summary>
	/// An <see cref="T:StringDB.IO.IDatabaseIODevice" /> that interfaces with <see cref="T:StringDB.IO.ILowlevelDatabaseIODevice" />
	/// </summary>
	public sealed class DatabaseIODevice : IDatabaseIODevice
	{
		private readonly ILowlevelDatabaseIODevice _lowlevelDBIOD;

		public DatabaseIODevice(ILowlevelDatabaseIODevice lowlevelDBIOD) => _lowlevelDBIOD = lowlevelDBIOD;

		public void Reset() => _lowlevelDBIOD.Reset();

		public byte[] ReadValue(long position)
		{
			// temporarily go to the position to read the value,
			// then seek back to the cursor position for reading
			var curPos = _lowlevelDBIOD.GetPosition();

			var value = _lowlevelDBIOD.ReadValue(position);

			_lowlevelDBIOD.Seek(curPos);

			return value;
		}

		public DatabaseItem ReadNext()
		{
			// handle EOFs/Jumps
			var peek = _lowlevelDBIOD.Peek();

			ExecuteJumps(ref peek);

			if (peek == NextItemPeek.EOF)
			{
				return new DatabaseItem
				{
					EndOfItems = true
				};
			}

			// peek HAS to be an Index at this point

			var item = _lowlevelDBIOD.ReadIndex();

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
				var jump = _lowlevelDBIOD.ReadJump();
				_lowlevelDBIOD.Seek(jump);
				peek = _lowlevelDBIOD.Peek();
			}
		}

		public void Insert(KeyValuePair<byte[], byte[]>[] items)
		{
			_lowlevelDBIOD.SeekEnd();

			var offset = _lowlevelDBIOD.GetPosition();

			UpdatePreviousJump(offset);

			// we need to calculate the total offset of all the indexes
			// then we write every index & increment the offset by the offset of each value
			// and then we write the values

			// phase 1: calculating total offset

			foreach (var kvp in items)
			{
				offset += _lowlevelDBIOD.CalculateIndexOffset(kvp.Key);
			}

			// the jump offset is important, we will be jumping after
			offset += _lowlevelDBIOD.JumpOffsetSize;

			// phase 2: writing each key
			//			and incrementing the offset by the value

			foreach (var kvp in items)
			{
				_lowlevelDBIOD.WriteIndex(kvp.Key, offset);

				offset += _lowlevelDBIOD.CalculateValueOffset(kvp.Value);
			}

			WriteJump();

			// phase 3: writing each value sequentially

			foreach (var kvp in items)
			{
				_lowlevelDBIOD.WriteValue(kvp.Value);
			}
		}

		private void UpdatePreviousJump(long jumpTo)
		{
			var currentPosition = _lowlevelDBIOD.GetPosition();

			if (_lowlevelDBIOD.JumpPos != 0)
			{
				// goto old jump pos and overwrite it with the current jump pos
				_lowlevelDBIOD.Seek(_lowlevelDBIOD.JumpPos);
				_lowlevelDBIOD.WriteJump(jumpTo);
			}

			_lowlevelDBIOD.Seek(currentPosition);
		}

		private void WriteJump()
		{
			var position = _lowlevelDBIOD.GetPosition();

			_lowlevelDBIOD.JumpPos = position;
			_lowlevelDBIOD.WriteJump(0);
		}

		public void Dispose()
		{
			_lowlevelDBIOD.Flush();
			_lowlevelDBIOD.Dispose();
		}
	}
}