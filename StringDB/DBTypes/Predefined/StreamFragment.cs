using System;
using System.IO;

namespace StringDB.DBTypes.Predefined {

	internal class StreamFragment : Stream {

		public StreamFragment(Stream main, long pos, long lenAfterPos) {
			this.Length = lenAfterPos;
			this._originalPos = pos;
			this._pos = pos;
			this._s = main;
		}

		private readonly Stream _s;

		public override bool CanRead => true;
		public override bool CanSeek => true;
		public override bool CanWrite => false;

		public override long Length { get; }

		private long _originalPos { get; }
		private long _pos;

		public override long Position {
			get => this._pos - this._originalPos;
			set {
				var lP = this._pos;
				this._pos = this._originalPos + value;
				//TODO:                                >=
				if (this.Position < 0 || this.Position > this.Length)
					this._pos = lP;
			}
		}

		public override void Flush() {
		}

		public override int Read(byte[] buffer, int offset, int count) {
			if (this.Position < 0)
				return -1;

			var c = count;
			if (this.Position + c > this.Length)
				c += (int)(this.Length - ((this.Position) + c));

			this._s.Seek(this._pos, SeekOrigin.Begin);
			this._pos += c;
			return this._s.Read(buffer, offset, c);
		}

		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
				this.Position = offset;
				break;

				case SeekOrigin.Current:
				this.Position += offset;
				break;

				case SeekOrigin.End:
				this.Position = this.Length + offset;
				break;

				default: break;
			}

			return this.Position;
		}

		public override void SetLength(long value) => throw new NotImplementedException();

		public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
	}
}