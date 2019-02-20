using System;
using System.IO;
using System.Text;

namespace StringDB.IO.Compatability
{
	public sealed class StringDB5_0_0LowlevelDatabaseIODevice : ILowlevelDatabaseIODevice
	{
		// https://github.com/SirJosh3917/StringDB/blob/fa703ed893b473829b6140f9d7033575d3291846/StringDB/Consts.cs
		private sealed class Consts
		{
			public const int MaxLength = 0xFE;
			public const byte DeletedValue = 0xFE;
			public const byte IndexSeperator = 0xFF;
			public const byte IsByteValue = 0x01;
			public const byte IsUShortValue = 0x02;
			public const byte IsUIntValue = 0x03;
			public const byte IsLongValue = 0x04;
			public const byte NoIndex = 0x00;
			public const int NOSPECIFYLEN = -1;

			// https://github.com/SirJosh3917/StringDB/blob/fa703ed893b473829b6140f9d7033575d3291846/StringDB/DBTypes/Predefined/ByteArrayWriterType.cs#L6
			public const byte ByteArrayTypeHandler = 0x01;
		}

		private readonly Stream _stream;
		private readonly BinaryReader _br;
		private readonly BinaryWriter _bw;

		// TODO: the first 8 bytes should be the last index chain, or whatever that is
		public StringDB5_0_0LowlevelDatabaseIODevice
		(
			Stream stream,
			bool leaveStreamOpen = true
		)
		{
			_stream = stream;
			_br = new BinaryReader(stream, Encoding.UTF8, leaveStreamOpen);
			_bw = new BinaryWriter(stream, Encoding.UTF8, leaveStreamOpen);
		}

		public long JumpPos { get; set; }

		public long GetPosition() => _stream.Position;

		public void Reset() => Seek(0);

		public void SeekEnd() => _stream.Seek(0, SeekOrigin.End);

		public void Seek(long position) => _stream.Seek(position, SeekOrigin.Begin);

		public void Flush()
		{
			_bw.Flush();
			_stream.Flush();
		}

		public NextItemPeek Peek()
		{
			switch (_br.PeekChar())
			{
				case -1:
				case Consts.NoIndex:
					return NextItemPeek.EOF;

				case Consts.IndexSeperator:
					return NextItemPeek.Jump;

				default:
					return NextItemPeek.Index;
			}
		}

		public LowLevelDatabaseItem ReadIndex()
		{
			var indexLength = _br.ReadByte();
			var dataPosition = _br.ReadInt64();
			var inputType = _br.ReadByte();

			// inputType is for TypeManager stuff in StringDB, we can throw it out of the window

			var index = _br.ReadBytes(indexLength);

			return new LowLevelDatabaseItem
			{
				Index = index,
				DataPosition = dataPosition
			};
		}

		public byte[] ReadValue(long dataPosition)
		{
			Seek(dataPosition);

			var length = ReadLength();

			return _br.ReadBytes(length);
		}

		public long ReadJump()
		{
			var jumpKey = _br.ReadByte();

			return _br.ReadInt64();
		}

		public void WriteJump(long jumpTo)
		{
			_bw.Write(Consts.IndexSeperator);
			_bw.Write(jumpTo);
		}

		public void WriteIndex(byte[] key, long dataPosition)
		{
			if (key.Length > Consts.MaxLength)
			{
				throw new IndexOutOfRangeException($"Index longer than {Consts.MaxLength}.");
			}

			_bw.Write((byte)key.Length);
			_bw.Write(dataPosition);
			_bw.Write(Consts.ByteArrayTypeHandler);
			_bw.Write(key);
		}

		public void WriteValue(byte[] value)
		{
			_bw.Write(Consts.ByteArrayTypeHandler);
			WriteLength(value.Length);
			_bw.Write(value);
		}

		public int CalculateIndexOffset(byte[] key)
			=> sizeof(byte)
			+ sizeof(long)
			+ sizeof(byte)
			+ key.Length;

		public int CalculateValueOffset(byte[] value)
			=> sizeof(byte)
			+ CalculateWriteLengthOffset(value.Length)
			+ value.Length;

		public int JumpOffsetSize { get; } = sizeof(byte) + sizeof(long);

		public void Dispose()
		{
			Flush();

			_br.Dispose();
			_bw.Dispose();
		}

		private int ReadLength()
		{
			var lengthIdentifier = _br.ReadByte();

			switch (lengthIdentifier)
			{
				case Consts.IsByteValue:
					return _br.ReadByte();

				case Consts.IsUShortValue:
					return _br.ReadUInt16();

				case Consts.IsUIntValue:
				{
					var length = _br.ReadUInt32();

					if (length > int.MaxValue)
					{
						throw new IndexOutOfRangeException($"{length} is bigger than the integer max");
					}

					return (int)length;
				}

				default: throw new IOException($"Didn't expect to read some length with byte identifier {lengthIdentifier}");
			}
		}

		private void WriteLength(int length)
		{
			if (length < byte.MaxValue)
			{
				_bw.Write(Consts.IsByteValue);
				_bw.Write((byte)length);
			}
			else if (length < ushort.MaxValue)
			{
				_bw.Write(Consts.IsUShortValue);
				_bw.Write((ushort)length);
			}
			else
			{
				_bw.Write(Consts.IsUIntValue);
				_bw.Write(length);
			}
		}

		private int CalculateWriteLengthOffset(int length)
		{
			if (length < byte.MaxValue)
			{
				return sizeof(byte) + sizeof(byte);
			}
			else if (length < ushort.MaxValue)
			{
				return sizeof(byte) + sizeof(ushort);
			}
			else
			{
				return sizeof(byte) + sizeof(uint);
			}
		}
	}
}