using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Reader
{
	/// <summary>An interface to signify a low-level reader that handles and manages the reading of data very quickly and efficiently.</summary>
    public interface IRawReader {

		/// <summary>Read some data at the exact position specified</summary>
		/// <param name="position">The position to go to when seeking</param>
		/// <returns>The result after seeking to the position and reading it</returns>
		IReadResult ReadAt(ulong position);

		/// <summary>Reads the very next result from the previous result</summary>
		/// <param name="previous">The previous result to use</param>
		/// <returns>The next result</returns>
		IReadResult ReadNext(IReadResult previous);
	}

	/// <summary>A class that does the heavy lifting of reading from a stream</summary>
	public class RawReader : IRawReader, IDisposable {

		/// <summary>Create a new RawReader</summary>
		/// <param name="stream">The stream to read from</param>
		/// <param name="dbv">The database version to keep in mind</param>
		/// <param name="leaveOpen">If the stream should be left open once done with</param>
		public RawReader(Stream stream, DatabaseVersion dbv, bool leaveOpen) {
			this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this._dbv = dbv;
			this._leaveOpen = leaveOpen;
			
#if NET20 || NET35 || NET40
			this._br = new System.IO.BinaryReader(this._stream, Encoding.UTF8);
#else
			this._br = new System.IO.BinaryReader(this._stream, Encoding.UTF8, leaveOpen);
#endif
		}

		private readonly object _lock = new object();
		private Stream _stream;
		private BinaryReader _br;
		private DatabaseVersion _dbv;
		private bool _leaveOpen;

		/// <inheritdoc/>
		public IReadResult ReadAt(ulong position) {
			lock(this._lock) {
				this.Seek(position);

				if (IsEndOfStream(sizeof(byte))) return new ReadEndOfStream();

				var b = this._br.ReadByte();

				switch(b) {

					//if we come across a special value
					case Consts.IndexSeperator:
					case Consts.DeletedValue: { //TODO: implement deleted values more better i guess
						if (IsEndOfStream(sizeof(ulong))) return new ReadEndOfStream();

						return new ReadIndexChain(position, this._br.ReadUInt64());
					}

					//else we've come across an index
					default: {
						if (IsEndOfStream(sizeof(ulong) + b)) return new ReadEndOfStream();

						var dataPos = this._br.ReadUInt64();
						var indexPos = (ulong)this._stream.Position;
						var indexName = this._br.ReadBytes(b);
						var next = (ulong)this._stream.Position;

						return new ReadDataPair(position, next, b, indexPos, dataPos, indexName);
					}
				}
			}
		}

		/// <inheritdoc/>
		public IReadResult ReadNext(IReadResult previous) => ReadAt((previous ?? throw new ArgumentNullException(nameof(previous))).QuickSeekToNext);

		/// <summary>Dispose stuff i guess...</summary>
		public void Dispose() {
			if(!this._leaveOpen) {
				this._stream.Dispose();
				this._br.Dispose();
			}
		}

		private bool IsEndOfStream(long bytesNeed = 1) =>
			(this._stream.Length - bytesNeed >= this._stream.Position);

		private void Seek(ulong pos) {
			if (pos < long.MaxValue) //support for really far data positions
				this._stream.Seek((long)pos, SeekOrigin.Begin);
			else {
				this._stream.Seek(long.MaxValue, SeekOrigin.Begin); //seek to the max value of long
				this._stream.Seek((long)(pos - (ulong)long.MaxValue), SeekOrigin.Current); //then even further
			}
		}
	}
}
