using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.IO.Compatability
{
	public class StringDB10_0_0LowlevelDatabaseIODevice : ILowlevelDatabaseIODevice
	{
		private static class Constants
		{
			public const byte IndexSeparator = 0xFF;
		}

		private readonly Stream _stream;
		private readonly BinaryReader _br;
		private readonly BinaryWriter _bw;

		public StringDB10_0_0LowlevelDatabaseIODevice(Stream stream, bool leaveStreamOpen = false)
		{
			_stream = stream;
			_br = new BinaryReader(_stream, Encoding.UTF8, leaveStreamOpen);
			_bw = new BinaryWriter(_stream, Encoding.UTF8, leaveStreamOpen);

			JumpPos = ReadBeginning();
		}

		private long ReadBeginning()
		{
			Seek(0);

			if (_stream.Length >= 8)
			{
				return _br.ReadInt64();
			}

			// we will create it if it doesn't exist
			_bw.Write(0L);
			return 0;

		}

		public int JumpOffsetSize { get; } = sizeof(byte) + sizeof(int);

		public long JumpPos { get; set; }

		public long GetPosition() => _stream.Position;
		public void Reset() => Seek(sizeof(long));
		public void Seek(long position) => _stream.Seek(position, SeekOrigin.Begin);
		public void SeekEnd() => _stream.Seek(0, SeekOrigin.End);

		public void Flush()
		{
			_bw.Flush();
			_stream.Flush();
		}

		public NextItemPeek Peek()
		{
			throw new NotImplementedException();
		}

		public LowLevelDatabaseItem ReadIndex()
		{
			throw new NotImplementedException();
		}

		public byte[] ReadValue(long dataPosition)
		{
			throw new NotImplementedException();
		}

		public long ReadJump()
		{
			throw new NotImplementedException();
		}

		public void WriteIndex(byte[] key, long dataPosition)
		{
			throw new NotImplementedException();
		}

		public void WriteValue(byte[] value)
		{
			throw new NotImplementedException();
		}

		public void WriteJump(long jumpTo)
		{
			throw new NotImplementedException();
		}

		public int CalculateIndexOffset(byte[] key)
		{
			throw new NotImplementedException();
		}

		public int CalculateValueOffset(byte[] value)
		{
			throw new NotImplementedException();
		}

		private int ReadIndexLength() => _br.ReadByte();

		private void WriteIndexLength(byte length)
		{
			if (length >= Constants.IndexSeparator)
			{
				throw new ArgumentException
				(
					$"The index length is too long - must be shorter than {Constants.IndexSeparator}",
					nameof(length)
				);
			}

			_bw.Write(length);
		}

		// https://wiki.vg/Data_types#VarInt_and_VarLong
		// the first bit tells us if we need to read more
		// the other 7 are used to encode the value

		private int ReadVariableLength()
		{
			var bytesRead = 0;
			var totalResult = 0;
			byte current;

			do
			{
				current = _br.ReadByte();
				var value = current & 0b01111111;

				totalResult |= value << 7 * bytesRead;

				bytesRead++;

				if (bytesRead > 5)
				{
					throw new FormatException("Not expected to read more than 5 numbers.");
				}

			}
			while ((current & 0b10000000) != 0);

			return totalResult;
		}

		private void WriteVariableLength(int value)
		{
			var currentValue = value;

			do
			{
				var read = currentValue & 0b01111111;

				currentValue >>= 7;

				if (currentValue != 0)
				{
					read |= 0b10000000;
				}

				_bw.Write(read);
			}
			while (currentValue != 0);
		}

		private int CalculateVariableByteSize(int value)
		{
			var result = 0;
			var currentValue = value;

			do
			{
				currentValue >>= 7;
				result++;
			}
			while (currentValue != 0);

			return result;
		}

		public void Dispose()
		{
			Flush();

			_bw.Dispose();
			_br.Dispose();
		}
	}
}
