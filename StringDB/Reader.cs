using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {
	public interface IReader : IEnumerable<string> {
		string GetValueOf(IReaderInteraction r, bool doSeek = false);
		string GetValueOf(string index, bool doSeek = false, ulong quickSeek = 0);

		string[] GetValuesOf(IReaderInteraction r, bool doSeek = false);
		string[] GetValuesOf(string index, bool doSeek = false, ulong quickSeek = 0);

		bool IsIndexAfter(IReaderInteraction r, bool doSeek = false);
		bool IsIndexAfter(string index, bool doSeek = false, ulong quickSeek = 0);

		IReaderInteraction IndexAfter(IReaderInteraction r, bool doSeek = false);
		IReaderInteraction IndexAfter(string index, bool doSeek = false, ulong quickSeek = 0);
		
		IReaderInteraction FirstIndex();

		string[] GetIndexes();

		IReaderChain GetReaderChain();
	}

	public class Reader : IReader {
		public Reader(Stream streamUse) {
			_stream = streamUse;
			_br = new BinaryReader(this._stream);
		}

		public const byte IndexSeperator = 0xFF;

		//public implementations of stuff

		#region public implementations
		public string[] GetIndexes() => _Indexes();

		public IEnumerator<string> GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex());
		IEnumerator IEnumerable.GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex());

		public string GetValueOf(IReaderInteraction r, bool doSeek = true) => GetValueOf(r.Index, doSeek, r.QuickSeek);
		public string GetValueOf(string index, bool doSeek = true, ulong quickSeek = 0) => _ValueOf(index, doSeek, quickSeek);

		public string[] GetValuesOf(IReaderInteraction r, bool doSeek = true) => GetValuesOf(r.Index, doSeek, r.QuickSeek);
		public string[] GetValuesOf(string index, bool doSeek = true, ulong quickSeek = 0) => _ValuesOf(index, doSeek, quickSeek);

		public bool IsIndexAfter(IReaderInteraction r, bool doSeek = true) => IsIndexAfter(r.Index, doSeek, r.QuickSeek);
		public bool IsIndexAfter(string index, bool doSeek = true, ulong quickSeek = 0) => _IsIndexAfter(index, doSeek, quickSeek);

		public IReaderInteraction IndexAfter(IReaderInteraction r, bool doSeek = true) => IndexAfter(r.Index, doSeek, r.QuickSeek);
		public IReaderInteraction IndexAfter(string index, bool doSeek = true, ulong quickSeek = 0) => _IndexAfter(index, doSeek, quickSeek);

		public IReaderInteraction FirstIndex() => IndexAfter(null, true, 0);

		public IReaderChain GetReaderChain() => _ReadChain();
		#endregion

		//nitty gritty part

		private Stream _stream { get; set; }
		private BinaryReader _br { get; set; }

		private string _ValueOf(string index, bool doSeek, ulong quickSeek) {
			var _curPos = _br.BaseStream.Position;

			var i = _ReadIndex(doSeek, quickSeek);

			while (i.Index != index) {
				i = _ReadIndex();
				if (i == null)
					return null;
			}

			return _ReadValue(i);
		}

		private string[] _ValuesOf(string index, bool doSeek, ulong quickSeek) {
			var _curPos = _br.BaseStream.Position;

			var i = _ReadIndex(doSeek, quickSeek);

			var valuesOf = new List<string>();

			var lastpos = _br.BaseStream.Position;

			if(i != null)
				if (i.Index == index)
					valuesOf.Add(_ReadValue(i));

			while (i.Index != null) {
				i = _ReadIndex(true, (ulong)lastpos);
				if (i == null)
					break;

				lastpos = _br.BaseStream.Position;

				if (i.Index == index)
					valuesOf.Add(_ReadValue(i));
			}

			return valuesOf.ToArray();
		}

		private bool _IsIndexAfter(string index, bool doSeek, ulong quickSeek) {
			var _curPos = _br.BaseStream.Position;

			var rs = _ReadIndex(doSeek, quickSeek);

			while (rs.Index != index) {
				rs = _ReadIndex();
				if (rs == null)
					return false;
			}

			rs = _ReadIndex();

			_br.BaseStream.Seek(_curPos, SeekOrigin.Begin);

			return rs == null;
		}

		private IReaderInteraction _IndexAfter(string index, bool doSeek, ulong quickSeek) {
			var _curPos = _br.BaseStream.Position;

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

			_br.BaseStream.Seek(_curPos, SeekOrigin.Begin);

			if (rs == null)
				return null;

			return rs;
		}

		private string[] _Indexes() {
			var _curPos = _br.BaseStream.Position;

			var indexes = new List<string>();

			var rs = _ReadIndex(true, 0);

			while (rs != null) {
				indexes.Add(rs.Index);
				rs = _ReadIndex();
			}

			_br.BaseStream.Seek(_curPos, SeekOrigin.Begin);

			return indexes.ToArray();
		}

		//the actual work part of it

		private IReaderInteraction _ReadIndex(bool restart = false, ulong start = 0) {
			if (restart)
				_br.BaseStream.Seek((long)start, SeekOrigin.Begin);

			byte b = _br.ReadByte();

			while (b == IndexSeperator) { //hippety hoppity get off my property
				var seekTo = (long)(_br.ReadUInt64());

				if (seekTo == 0)
					return null;

				_br.BaseStream.Seek(seekTo, SeekOrigin.Begin);
				b = _br.ReadByte();
			}

			var dataPos = _br.ReadUInt64();
			var indexName = Encoding.UTF8.GetString(_br.ReadBytes((int)b));

			return new ReaderInteraction(
					indexName, (ulong)_br.BaseStream.Position, dataPos
				);
		}

		private string _ReadValue(IReaderInteraction readerInteraction) {
			_br.BaseStream.Seek((long)readerInteraction.DataPos, SeekOrigin.Begin);

			return Encoding.UTF8.GetString(
					_br.ReadBytes(_br.ReadInt32())
				);
		}

		private string _ReadValue(ulong location) {
			_br.BaseStream.Seek((long)location, SeekOrigin.Begin);

			return Encoding.UTF8.GetString(
					_br.ReadBytes(_br.ReadInt32())
				);
		}

		private IReaderChain _ReadChain() {
			_br.BaseStream.Seek((long)0, SeekOrigin.Begin);

			ulong ic = 0;
			ulong icw = 0;

			bool shouldContinueLook = true;
			byte b = _br.ReadByte();

			while (shouldContinueLook) {
				while (b == IndexSeperator) { //hippety hoppity get off my property
					var p = _br.BaseStream.Position;
					var seekTo = (long)(_br.ReadUInt64());

					if (seekTo == 0)
						shouldContinueLook = false;
					else {
						ic = (ulong)seekTo;
						icw = (ulong)p;

						_br.BaseStream.Seek(seekTo, SeekOrigin.Begin);
					}

					b = _br.ReadByte();
				}

				//stuff to pass the reader
				_br.ReadUInt64();
				_br.ReadBytes((int)b);

				if (_br.BaseStream.Position == _br.BaseStream.Length)
					break;

				b = _br.ReadByte();
			}

			return new ReaderChain(ic, icw);
		}
	}

	public class ReaderEnumerator : IEnumerator<string> {
		internal ReaderEnumerator(Reader parent, IReaderInteraction start) {
			this._parent = parent;

			this._indexOn = start.Index;
			
			this._seekTo = 0;
			this._toSeek = start.QuickSeek;
			this._first = false;
		}

		private bool _first { get; set; }

		private string _indexOn { get; set; }
		private ulong _seekTo { get; set; }
		private ulong _toSeek { get; set; }
		private Reader _parent { get; set; }

		public string Current => _parent.GetValueOf(this._indexOn, true, this._seekTo);

		object IEnumerator.Current => Current;

		public bool MoveNext() {
			if (!this._first) {
				this._first = true;
				return true;
			}

			var rr = _parent.IndexAfter(this._indexOn, true, this._seekTo);

			if (rr == null)
				return false;

			this._indexOn = rr.Index;
			
			this._seekTo = this._toSeek;
			this._toSeek = rr.QuickSeek;

			return true;
		}

		public void Reset() {
			var rr = _parent.FirstIndex();

			this._indexOn = rr.Index;
			this._seekTo = rr.QuickSeek;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ReaderEnumerator() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}

	//
	
	public interface IReaderChain {
		ulong IndexChain { get; }
		ulong IndexChainWrite { get; }
	}

	public struct ReaderChain : IReaderChain {
		public ReaderChain(ulong indexChain, ulong indexChainWrite) {
			this.IndexChain = indexChain;
			this.IndexChainWrite = indexChainWrite;
		}

		public ulong IndexChain { get; }
		public ulong IndexChainWrite { get; }
	}
	
	public interface IReaderInteraction {
		string Index { get; }
		ulong QuickSeek { get; }
		ulong DataPos { get; }
	}

	public struct ReaderInteraction : IReaderInteraction {
		public ReaderInteraction(string index, ulong quickSeek = 0, ulong dataPos = 0) {
			this.Index = index;
			this.QuickSeek = quickSeek;
			this.DataPos = dataPos;
		}

		public string Index { get; }
		public ulong QuickSeek { get; }
		public ulong DataPos { get; }
	}
}