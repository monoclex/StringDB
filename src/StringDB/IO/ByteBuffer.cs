using System.IO;

namespace StringDB.IO
{
	public sealed class NoByteBuffer
	{
		public static byte[] Read(BinaryReader reader, int count)
			=> reader.ReadBytes(count);
	}

	/// <summary>
	/// Offers buffering read results to prevent newing up byte arrays constantly
	/// for sizes less than 256
	/// </summary>
	public sealed class ByteBuffer
	{
		public ByteBuffer()
		{
			_buffers = new byte[byte.MaxValue + 1][];

			for (var i = 0; i <= byte.MaxValue; i++)
			{
				_buffers[i] = new byte[i];
			}
		}

		private readonly byte[][] _buffers;

		public byte[] Read(BinaryReader reader, int count)
		{
			var array = _buffers[count];

			reader.Read(array, 0, count);

			return array;
		}
	}
}