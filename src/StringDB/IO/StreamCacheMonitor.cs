using System;

using JetBrains.Annotations;

using System.IO;

namespace StringDB.IO
{
	/// <summary>
	/// Caches a position and length of the underlying stream,
	/// to make position and length lookups quick and snappy.
	/// </summary>
	[PublicAPI]
	public class StreamCacheMonitor : Stream
	{
		[NotNull] private readonly Stream _stream;

		public StreamCacheMonitor([NotNull] Stream stream)
		{
			_stream = stream;
			_pos = _stream.Position;
			_len = _stream.Length;
		}

		private long _pos;

		private long _len;

		public override void Flush()
			=> _stream.Flush();

		public override long Seek(long offset, SeekOrigin origin)
		{
			_pos = _stream.Seek(offset, origin);

			return _pos;
		}

		public override void SetLength(long value)
		{
			_len = value;
			_stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var result = _stream.Read(buffer, offset, count);
			_pos += result;

			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_pos += count;

			if (_pos > _len)
			{
				_len = _pos;
			}

			_stream.Write(buffer, offset, count);
		}

		public override bool CanRead => _stream.CanRead;
		public override bool CanSeek => _stream.CanSeek;
		public override bool CanWrite => _stream.CanWrite;

		public override long Length => _len;

		public override long Position
		{
			get => _pos;
			set
			{
				_pos = value;
				_stream.Position = value;
			}
		}

		public override void Close()
		{
			_stream.Close();
		}

		public void UpdateCache()
		{
			_pos = _stream.Position;
			_len = _stream.Length;
		}
	}
}