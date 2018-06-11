using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Reader {
	/// <inheritdoc/>
	public class StreamReader : IReader {
		/// <summary>Create a new StreamReader.</summary>
		/// <param name="streamUse">The stream to read . You may need to call the Load() void to set the indexChain data.</param>
		/// <param name="dbv">The database version to read from.</param>
		/// <param name="keepStreamOpen">Whether or not the stream is to be disposed after using it.</param>
		public StreamReader(Stream streamUse, DatabaseVersion dbv, bool keepStreamOpen) {
			this._stream = streamUse;
#if NET20 || NET35 || NET40
			this._br = new BinaryReader(this._stream, System.Text.Encoding.UTF8);
#else
			this._br = new BinaryReader(this._stream, System.Text.Encoding.UTF8, keepStreamOpen);
#endif
			this._dbv = dbv;
			this._keepStreamOpen = keepStreamOpen;
		}

		private DatabaseVersion _dbv;
		private bool _keepStreamOpen;

		//public implementations of stuff

		#region public implementations

		/// <inheritdoc/>
		public ulong GetOverhead() => _GetOverhead(); /// <inheritdoc/>
		public byte[][] GetIndexes() => _Indexes(); /// <inheritdoc/>

		public IEnumerator<ReaderPair> GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex());/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex());/// <inheritdoc/>

		public byte[] GetValueOf(IReaderInteraction r, bool doSeek = true) => GetValueOf(r.Index, doSeek, r.QuickSeek);/// <inheritdoc/>
		public byte[] GetValueOf(string index, bool doSeek = true, ulong quickSeek = 0) => _ValueOf(index, doSeek, quickSeek);/// <inheritdoc/>
			
		public byte[] GetDirectValueOf(ulong dataPos) => _DirectValueOf(dataPos);/// <inheritdoc/>

		public byte[][] GetValuesOf(IReaderInteraction r, bool doSeek = true) => GetValuesOf(r.Index, doSeek, r.QuickSeek);/// <inheritdoc/>
		public byte[][] GetValuesOf(string index, bool doSeek = true, ulong quickSeek = 0) => _ValuesOf(index, doSeek, quickSeek);/// <inheritdoc/>

		public bool IsIndexAfter(IReaderInteraction r, bool doSeek = true) => IsIndexAfter(r.Index, doSeek, r.QuickSeek);/// <inheritdoc/>
		public bool IsIndexAfter(string index, bool doSeek = true, ulong quickSeek = 0) => _IsIndexAfter(index, doSeek, quickSeek);/// <inheritdoc/>

		public IReaderInteraction IndexAfter(IReaderInteraction r, bool doSeek = true) => IndexAfter(r.Index, doSeek, r.QuickSeek);/// <inheritdoc/>
		public IReaderInteraction IndexAfter(string index, bool doSeek = true, ulong quickSeek = 0) => _IndexAfter(index, doSeek, quickSeek);/// <inheritdoc/>

		public IReaderInteraction FirstIndex() => IndexAfter(null, true, 0);/// <inheritdoc/>

		public IReaderChain GetReaderChain() => _ReadChain();
		#endregion

		//nitty gritty part

		private Stream _stream;
		private BinaryReader _br;

		//TODO: somehow simplify all of these functions - they're very similar and or identical

		private ulong _GetOverhead() {
			ulong overhead = 0;

			foreach (var i in this) {
				overhead += i._indexchainPassTimes * 9;

				overhead += 9;

				if ((int)this._dbv >= (int)DatabaseVersion.Version200) {
					this._br.BaseStream.Seek((long)i._dataPos.DataPos, SeekOrigin.Begin);

					var b = this._br.BaseStream.ReadByte();

					var doLoop = true;

					while (doLoop)
						switch (b) {
							case Consts.IsByteValue: {
								overhead += 2;
								doLoop = false;
							}
							break;
							case Consts.IsUShortValue: {
								overhead += 3;
								doLoop = false;
							}
							break;
							case Consts.IsUIntValue: {
								overhead += 5;
								doLoop = false;
							}
							break;
							case Consts.IsULongValue: {
								overhead += 9;
								doLoop = false;
							}
							break;
							case Consts.IndexSeperator: {
								b = this._br.ReadBytes(9)[8];
								doLoop = true;
							}
							break;
							default: {
								doLoop = false;
								throw new Exception("Seeking to the data leads to finding nothing");
							}
						}
				} else overhead += 4;
			}

			return overhead + 9;
		}

		private byte[] _DirectValueOf(ulong pos) {
			this._br.BaseStream.Seek((long)pos, SeekOrigin.Begin);
			return this._br.ReadBytes((int)GetNumber());
		}

		private byte[] _ValueOf(string index, bool doSeek, ulong quickSeek) {
			if (index == null)
				throw new ArgumentNullException("index");

			var _curPos = this._br.BaseStream.Position;

			var i = _ReadIndex(doSeek, quickSeek);

			if (i == null)
				return null;

			while (i.Index != index) {
				i = _ReadIndex();
				if (i == null)
					return null;
			}

			return _ReadValue(i);
		}

		private byte[][] _ValuesOf(string index, bool doSeek, ulong quickSeek) {
			if (index == null)
				throw new ArgumentNullException("index");

			var _curPos = this._br.BaseStream.Position;

			var i = _ReadIndex(doSeek, quickSeek);

			var valuesOf = new List<byte[]>();

			var lastpos = this._br.BaseStream.Position;

			if(i != null)
				if (i.Index == index)
					valuesOf.Add(_ReadValue(i));

			while (i.Index != null) {
				i = _ReadIndex(true, (ulong)lastpos);
				if (i == null)
					break;

				lastpos = this._br.BaseStream.Position;

				if (i.Index == index)
					valuesOf.Add(_ReadValue(i));
			}

			return valuesOf.ToArray();
		}

		private bool _IsIndexAfter(string index, bool doSeek, ulong quickSeek) {
			if (index == null)
				throw new ArgumentNullException("index");

			var _curPos = this._br.BaseStream.Position;

			var rs = _ReadIndex(doSeek, quickSeek);

			while (rs.Index != index) {
				rs = _ReadIndex();
				if (rs == null)
					return false;
			}

			rs = _ReadIndex();

			this._br.BaseStream.Seek(_curPos, SeekOrigin.Begin);

			return rs == null;
		}

		private IReaderInteraction _IndexAfter(string index, bool doSeek, ulong quickSeek) {
			var _curPos = this._br.BaseStream.Position;

			var rs = _ReadIndex(doSeek, quickSeek);

			if (rs == null)
				return null;

			if (index == null)
				return rs;

			while (rs.Index != index) {
				rs = _ReadIndex();
				if (rs == null)
					return null;
			}

			rs = _ReadIndex();

			this._br.BaseStream.Seek(_curPos, SeekOrigin.Begin);

			if (rs == null)
				return null;

			return rs;
		}

		private byte[][] _Indexes() {
			var _curPos = this._br.BaseStream.Position;

			var indexes = new List<byte[]>();

			var rs = _ReadIndex(true, 0);

			while (rs != null) {
				indexes.Add(
					Database.GetBytes(rs.Index)
					);
				rs = _ReadIndex();
			}

			this._br.BaseStream.Seek(_curPos, SeekOrigin.Begin);

			return indexes.ToArray();
		}

		//the actual work part of it

		private IReaderInteraction _ReadIndex(bool restart = false, ulong start = 0) {
			if (restart)
				this._br.BaseStream.Seek((long)start, SeekOrigin.Begin);

			var b = this._br.ReadByte();
			var passedIndexChain = 0u;

			while (b == Consts.IndexSeperator) { //hippety hoppity get off my property
				passedIndexChain++;

				var seekTo = (long)(this._br.ReadUInt64());

				if (seekTo == 0)
					return null;

				this._br.BaseStream.Seek(seekTo, SeekOrigin.Begin);
				b = this._br.ReadByte();
			}

			var dataPos = this._br.ReadUInt64();
			
			var indexName = this._br.ReadBytes((int)b);

			return new ReaderInteraction(
					Database.GetString(indexName), dataPos, (ulong)this._br.BaseStream.Position, passedIndexChain
				);
		}

		private byte[] _ReadValue(IReaderInteraction readerInteraction) {
			if (readerInteraction == null)
				throw new ArgumentNullException("readerInteraction");

			this._br.BaseStream.Seek((long)readerInteraction.DataPos, SeekOrigin.Begin);

			return this._br.ReadBytes((int)this.GetNumber());
		}

		private byte[] _ReadValue(ulong location) {
			this._br.BaseStream.Seek((long)location, SeekOrigin.Begin);

			return this._br.ReadBytes((int)this.GetNumber());
		}

		private IReaderChain _ReadChain() {
			this._br.BaseStream.Seek((long)0, SeekOrigin.Begin);

			ulong ic = 0;
			ulong icw = 0;

			var shouldContinueLook = true;
			var b = this._br.ReadByte();

			while (shouldContinueLook) {
				while (b == Consts.IndexSeperator) { //hippety hoppity get off my property
					var p = this._br.BaseStream.Position;
					var seekTo = (long)(this._br.ReadUInt64());

					if (seekTo == 0) {
						icw = (ulong)p;// (ulong)p - 8;
						ic = (ulong)this._br.BaseStream.Length;
						shouldContinueLook = false;
						break;
					} else {
						ic = (ulong)seekTo;
						icw = (ulong)p;

						this._br.BaseStream.Seek(seekTo, SeekOrigin.Begin);
					}

					b = this._br.ReadByte();
				}

				if (!shouldContinueLook)
					break;

				if (this._br.BaseStream.Position >= this._br.BaseStream.Length)
					break;

				//stuff to pass the reader
				this._br.ReadUInt64();
				this._br.ReadBytes((int)b);

				if (this._br.BaseStream.Position >= this._br.BaseStream.Length)
					break;

				b = this._br.ReadByte();
			}

			return new ReaderChain(ic, icw);
		}

		private ulong GetNumber() {
			if ((int)this._dbv >= (int)DatabaseVersion.Version200) {
				var b = this._br.ReadByte();

				switch (b) {
					case Consts.IsByteValue:
					return (ulong)this._br.ReadByte();
					case Consts.IsUShortValue:
					return (ulong)this._br.ReadUInt16();
					case Consts.IsUIntValue:
					return (ulong)this._br.ReadUInt32();
					case Consts.IsULongValue:
					return this._br.ReadUInt64();
				}

				throw new Exception("Invalid number");
			} else return (ulong)this._br.ReadInt32();
		}

		#region IDisposable Support
		private bool disposedValue = false;

		/// <summary>Dispose this object</summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing) {
			if (!this.disposedValue) {
				if (disposing) {
					if(!this._keepStreamOpen)
						this._stream.Dispose();

#if NET20 || NET35 || NET40
					if(!this._keepStreamOpen)
# endif
					((IDisposable)this._br).Dispose();
				}

				this._stream = null;
				this._br = null;

				this._dbv = DatabaseVersion.Version100;
				this._keepStreamOpen = true;

				this.disposedValue = true;
			}
		}
		
		/// <summary>Finalize this object</summary>
		~StreamReader() {
			Dispose(false);
		}

		/// <summary>Dispose this object</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
#endregion
	}
}