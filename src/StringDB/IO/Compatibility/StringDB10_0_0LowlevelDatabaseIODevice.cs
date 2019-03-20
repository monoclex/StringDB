using JetBrains.Annotations;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace StringDB.IO.Compatibility
{
	public sealed class StringDB10_0_0LowlevelDatabaseIODevice : ILowlevelDatabaseIODevice
	{
		private static class Constants
		{
			public const byte IndexSeparator = 0xFF;
		}

		private readonly StreamCacheMonitor _stream;
		private readonly BinaryReader _br;
		private readonly BinaryWriter _bw;

		public StringDB10_0_0LowlevelDatabaseIODevice
		(
			[NotNull] Stream stream,
			bool leaveStreamOpen = false
		)
		{
			// We wrap the stream in this so that lookups to Position and Length are quick and snappy.
			// This is to prevent a performance concern regarding EOF using excessive amounts of time.
			// This has the issue of being cached, but calling IODevice.Reset should fix it right up.
			// Of course, this has bad implications and might be reverted later, but it definitely
			// fixes the performance gap without making the code ugly.
			_stream = new StreamCacheMonitor(stream);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public long GetPosition() => _stream.Position;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_stream.UpdateCache();
			Seek(sizeof(long));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Seek(long position) => _stream.Seek(position, SeekOrigin.Begin);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SeekEnd() => _stream.Seek(0, SeekOrigin.End);

		public void Flush()
		{
			_bw.Flush();
			_stream.Flush();
		}

		public NextItemPeek Peek()
		{
			switch (PeekByte())
			{
				case Constants.IndexSeparator:
					return NextItemPeek.Jump;

				case 0x00:
					return NextItemPeek.EOF;

				default:
					return NextItemPeek.Index;
			}
		}

		private byte PeekByte()
		{
			if (EOF)
			{
				return 0x00;
			}

			var peek = _br.ReadByte();
			_stream.Position--;
			return peek;
		}

		public LowLevelDatabaseItem ReadIndex()
		{
			if (EOF)
			{
				throw new NotSupportedException("Cannot read past EOF.");
			}

			var length = ReadIndexLength();
			var dataPosition = ReadDownsizedLong();
			var index = _br.ReadBytes(length);

			return new LowLevelDatabaseItem
			{
				DataPosition = dataPosition,
				Index = index
			};
		}

		// As it turns out, this is a major performance concern
		// when using a FileStream.
		// Thus, the chosen solution was to wrap the given Stream into a StreamCacheMonitor.
		// This makes these lookups quick and snappy.
		private bool EOF => GetPosition() >= _stream.Length;

		public byte[] ReadValue(long dataPosition)
		{
			Seek(dataPosition);
			var length = ReadVariableLength();

			if (length > int.MaxValue)
			{
				throw new NotSupportedException($"Cannot read a value outside the integer bounds: {length}");
			}

			return _br.ReadBytes((int)length);
		}

		public long ReadJump()
		{
			var separator = _br.ReadByte();

			if (separator != Constants.IndexSeparator)
			{
				throw new NotSupportedException(
					$"Expected to read a {Constants.IndexSeparator}, but got {separator:x2} instead.");
			}

			return ReadDownsizedLong();
		}

		public void WriteIndex(byte[] key, long dataPosition)
		{
			_bw.Write(GetIndexSize(key.Length));
			_bw.Write(GetJumpSize(dataPosition));
			_bw.Write(key);
		}

		public void WriteValue(byte[] value)
		{
			WriteVariableLength((uint)value.Length);
			_bw.Write(value);
		}

		public void WriteJump(long jumpTo)
		{
			_bw.Write(Constants.IndexSeparator);

			// this is to cope with the DatabaseIODevice
			// it's pretty much a hacky workaround :v

			_bw.Write(jumpTo == 0 ? 0u : GetJumpSize(jumpTo));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CalculateIndexOffset(byte[] key)
			=> sizeof(byte)
			+ sizeof(int)
			+ key.Length;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CalculateValueOffset(byte[] value)
			=> CalculateVariableSize((uint)value.Length)
			+ value.Length;

		private long ReadDownsizedLong() => GetPosition() + _br.ReadInt32();

		private int ReadIndexLength() => _br.ReadByte();

		private byte GetIndexSize(int length)
		{
			if (length >= Constants.IndexSeparator)
			{
				throw new ArgumentException($"Didn't expect length to be longer than {Constants.IndexSeparator}", nameof(length));
			}

			if (length < 1)
			{
				throw new ArgumentException("Didn't expect length shorter than 1", nameof(length));
			}

			return (byte)length;
		}

		private uint GetJumpSize(long jumpTo)
		{
			var result = jumpTo - GetPosition();

			if (result > uint.MaxValue || result < uint.MinValue)
			{
				throw new ArgumentException
				(
					$"Attempting to jump too far: {jumpTo}, and currently at {GetPosition()} (resulting in a jump of {result})",
					nameof(result)
				);
			}

			return (uint)result;
		}

		// https://wiki.vg/Data_types#VarInt_and_VarLong
		// the first bit tells us if we need to read more
		// the other 7 are used to encode the value

		private uint ReadVariableLength()
		{
			var bytesRead = 0;
			var totalResult = 0u;
			byte current;

			do
			{
				current = _br.ReadByte();
				var value = (uint)(current & 0b01111111);

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

		private void WriteVariableLength(uint value)
		{
			var currentValue = value;

			do
			{
				var read = (byte)(currentValue & 0b01111111);

				currentValue >>= 7;

				if (currentValue != 0)
				{
					read |= 0b10000000;
				}

				_bw.Write(read);
			}
			while (currentValue != 0);
		}

		private int CalculateVariableSize(uint value)
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
			Seek(0);

			// write the jump position at the beginning
			_bw.Write(JumpPos);

			Flush();

			_bw.Dispose();
			_br.Dispose();
		}
	}
}