using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Reader
{
	/// <summary>An interface to signify a low-level reader that handles and manages the reading of data very quickly and efficiently.</summary>
    public interface IRawReader : IDisposable {

		/// <summary>Read some data at the exact position specified</summary>
		/// <param name="position">The position to go to when seeking</param>
		/// <returns>The result after seeking to the position and reading it</returns>
		IReadResult ReadAt(ulong position);

		/// <summary>Reads the very next result from the previous result</summary>
		/// <param name="previous">The previous result to use</param>
		/// <returns>The next result</returns>
		IReadResult ReadNext(IReadResult previous);

		/// <summary>Reads out the overhead of a datapair</summary>
		/// <param name="dataPair">The data pair to use</param>
		/// <returns>A ulong with the amount of overhead it uses</returns>
		ulong OverheadOf(IReadDataPair dataPair);
	}

	/// <summary>A class that does the heavy lifting of reading from a stream</summary>
	public class RawReader : IRawReader {

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
		public IReadResult ReadNext(IReadResult previous) => ReadAt((previous ?? throw new ArgumentNullException(nameof(previous))).QuickSeekToNext);

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
						//var indexName = this._br.ReadBytes(b);
						var next = (ulong)this._stream.Position + b; 

						return new ReadDataPair(position, next, b, indexPos, dataPos);
					}
				}
			}
		}

		/// <inheritdoc/>
		public ulong OverheadOf(IReadDataPair dataPair) {
			lock(this._lock) {
				this.Seek(dataPair.DataPosition);

				if (IsEndOfStream(sizeof(byte))) return 0;

				var b = this._br.ReadByte();

				switch(b) {
					case Consts.IsByteValue:
					return sizeof(byte) + sizeof(byte);

					case Consts.IsUShortValue:
					return sizeof(byte) + sizeof(ushort);

					case Consts.IsUIntValue:
					return sizeof(byte) + sizeof(uint);

					case Consts.IsULongValue:
					return sizeof(byte) + sizeof(ulong);
				}

				return 0;
			}
		}

		/// <summary>Dispose stuff i guess...</summary>
		public void Dispose() {
			if(!this._leaveOpen) {
				this._stream.Dispose();
				((IDisposable)this._br).Dispose();
			}
		}

		/// <param name="readType">Set this to a "byte array" or "string", or set it to the stream to read into</param>
		private IReadResult ReadValue(ulong pos, object readType) {
			lock (this._lock) {
				this.Seek(pos);

				if (IsEndOfStream(sizeof(byte))) return new ReadEndOfStream();

				var b = this._br.ReadByte();
				ulong length;

				switch (b) {
					case Consts.IsByteValue:
					if (IsEndOfStream(sizeof(byte))) return new ReadEndOfStream();
					length = this._br.ReadByte();
					break;

					case Consts.IsUShortValue:
					if (IsEndOfStream(sizeof(ushort))) return new ReadEndOfStream();
					length = this._br.ReadUInt16();
					break;

					case Consts.IsUIntValue:
					if (IsEndOfStream(sizeof(uint))) return new ReadEndOfStream();
					length = this._br.ReadUInt32();
					break;

					case Consts.IsULongValue:
					if (IsEndOfStream(sizeof(ulong))) return new ReadEndOfStream();
					length = this._br.ReadUInt64();
					break;

					default: throw new Exception("Unable to read the length of the data.");
				}

				if (readType is ReadType)
					switch ((ReadType)readType) {
						case ReadType.ByteArray: return new ReadData(this._br.ReadBytes(length > int.MaxValue ? throw new Exception($"Length of the data is bigger then int.MaxValue. Please use a FileStream to read out the data of this value.") : (int)length));
						case ReadType.String: return new ReadData(Database.GetString(this._br.ReadBytes(length > int.MaxValue ? throw new Exception($"Length of the data is bigger then int.MaxValue. Please use a FileStream to read out the data of this value.") : (int)length)));

						default: throw new ArgumentException($"{nameof(readType)} can not be of ReadType.Stream");
					} else if (readType is Stream) {
					var s = readType as Stream;
					var bufferLength = 81920;
					var buffer = new byte[bufferLength];

					int read;
					while ((read = this._stream.Read(buffer, 0, bufferLength)) > 0) {
						s.Write(buffer, 0, read);
					}

					return new ReadData(null);
				} else throw new ArgumentException($"{nameof(readType)} must be a {nameof(ReadType)} or a {nameof(Stream)}");
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

		private ulong ReadLength(byte b) =>
			(b == Consts.IsByteValue ?
				this._br.ReadByte()
				: b == Consts.IsUShortValue ?
					this._br.ReadUInt16()
					: b == Consts.IsUIntValue ?
						this._br.ReadUInt32()
						: b == Consts.IsULongValue ?
							this._br.ReadUInt64()
							: throw new Exception("Unable to read number.") );
	}

	/// <summary>ReadType - specify the type to read. Stream goes unused</summary>
	public enum ReadType {
		/// <summary>To read as a ByteArray</summary>
		ByteArray,

		/// <summary>To read as a String</summary>
		String,

		/// <summary>Do not use this.</summary>
		Stream
	}
}
