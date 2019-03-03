using JetBrains.Annotations;

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace StringDB.IO
{
	/// <inheritdoc />
	/// <summary>
	/// An IDatabaseIODevice that implements the StoneVault protocol.
	/// </summary>
	[PublicAPI]
	public sealed class StoneVaultIODevice : IDatabaseIODevice
	{
		private static class Consts
		{
			public const byte DATA_GOOD = 0x00;
			public const byte DATA_END = 0xFF;
		}

		private readonly Stream _stream;
		private readonly BinaryReader _br;
		private readonly BinaryWriter _bw;

		public StoneVaultIODevice
		(
			Stream stream,
			bool leaveStreamOpen = false
		)
		{
			_stream = stream;
			_br = new BinaryReader(stream, Encoding.UTF8, leaveStreamOpen);
			_bw = new BinaryWriter(stream, Encoding.UTF8, leaveStreamOpen);
		}

		public DatabaseItem ReadNext()
		{
			if (ShouldEndRead())
			{
				return End();
			}

			var key = ReadByteArray();

			// advance past the data but keep record of where it is
			var dataPosition = _stream.Position;

			if (ShouldEndRead())
			{
				return End();
			}

			var valueLength = _br.ReadInt64();
			_stream.Position += valueLength;

			return new DatabaseItem
			{
				Key = key,
				DataPosition = dataPosition
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static DatabaseItem End()
			=> new DatabaseItem { EndOfItems = true };

		public void Insert(KeyValuePair<byte[], byte[]>[] items)
		{
			// we will overwrite the DATA_END with a DATA_GOOD
			SeekOneBeforeEnd();

			foreach (var item in items)
			{
				WriteByteArray(item.Key);
				WriteByteArray(item.Value);
			}

			_bw.Write(Consts.DATA_END);
		}

		private void SeekOneBeforeEnd()
		{
			if (_stream.Length < 1)
			{
				_stream.Seek(0, SeekOrigin.Begin);
			}
			else
			{
				_stream.Seek(-1, SeekOrigin.End);
			}
		}

		private bool ShouldEndRead()
			=> _stream.Position >= _stream.Length - 1
			|| _br.ReadByte() == Consts.DATA_END;

		private void WriteByteArray(byte[] data)
		{
			_bw.Write(Consts.DATA_GOOD);
			_bw.Write((long)data.Length);
			_bw.Write(data);
		}

		private byte[] ReadByteArray()
		{
			// assume the DATA_GOOD/DATA_END has already been read
			var length = _br.ReadInt64();
			return _br.ReadBytes((int)length);
		}

		public byte[] ReadValue(long position)
		{
			var currentPos = _stream.Position;

			_stream.Seek(position, SeekOrigin.Begin);

			if (ShouldEndRead()) return new byte[0];

			var dataLength = _br.ReadInt64();

			var value = _br.ReadBytes((int)dataLength);

			_stream.Seek(currentPos, SeekOrigin.Begin);

			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset() => _stream.Seek(0, SeekOrigin.Begin);

		public void Dispose()
		{
			_bw.Flush();
			_stream.Flush();

			_br.Dispose();
			_bw.Dispose();
		}
	}
}