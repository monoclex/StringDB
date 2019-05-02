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
		[NotNull] public Stream InnerStream { get; }

		public StreamCacheMonitor([NotNull] Stream stream)
		{
			InnerStream = stream;
			_pos = InnerStream.Position;
			_len = InnerStream.Length;
		}

		private long _pos;

		private long _len;

		public override void Flush()
			=> InnerStream.Flush();

		public override long Seek(long offset, SeekOrigin origin)
		{
			_pos = InnerStream.Seek(offset, origin);

			return _pos;
		}

		public override void SetLength(long value)
		{
			_len = value;
			InnerStream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var result = InnerStream.Read(buffer, offset, count);
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

			InnerStream.Write(buffer, offset, count);
		}

		public override bool CanRead => InnerStream.CanRead;
		public override bool CanSeek => InnerStream.CanSeek;
		public override bool CanWrite => InnerStream.CanWrite;

		public override long Length => _len;

		public override long Position
		{
			get => _pos;
			set
			{
				_pos = value;
				InnerStream.Position = value;
			}
		}

		public override void Close()
		{
			InnerStream.Close();
		}

		public void UpdateCache()
		{
			_pos = InnerStream.Position;
			_len = InnerStream.Length;
		}
	}
}