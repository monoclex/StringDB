using JetBrains.Annotations;
using System;
using System.IO;

namespace StringDB.IO
{
	/// <summary>
	/// Caches a position and length of the underlying stream,
	/// to make position and length lookups quick and snappy.
	/// </summary>
	[PublicAPI]
	public sealed class StreamCacheMonitor : Stream
	{
		[NotNull] public Stream InnerStream { get; }

		public StreamCacheMonitor([NotNull] Stream stream)
		{
			InnerStream = stream;
			_pos = InnerStream.Position;
			_len = InnerStream.Length;
		}

		private long _pos;
		private long _userPos;
		private long _len;

		public override void Flush()
			=> InnerStream.Flush();

		[Obsolete("Please use the Position property in combination with the Length property for any kind of seeking.", true)]
		public override long Seek(long offset, SeekOrigin origin)
		{
			_pos = InnerStream.Seek(offset, origin);
			_userPos = _pos;

			return _pos;
		}

		public override void SetLength(long value)
		{
			_len = value;
			InnerStream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			ForceSeek();

			var result = InnerStream.Read(buffer, offset, count);
			_userPos = _pos += result;

			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ForceSeek();

			_userPos = _pos += count;

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
			get => _userPos;
			set
			{
				_userPos = value;
				InnerStream.Position = value;
			}
		}

		public override void Close()
		{
			InnerStream.Close();
		}

		public void UpdateCache()
		{
			_userPos = _pos = InnerStream.Position;
			_len = InnerStream.Length;
		}

		public void ForceSeek()
		{
			if (_userPos != _pos)
			{
				_userPos = _pos = InnerStream.Seek(_userPos, SeekOrigin.Begin);
			}
		}
	}
}