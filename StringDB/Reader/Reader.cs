using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StringDB.Reader {
	public interface IReader : IEnumerable<IReaderPair> {
		/// <summary>Gets the very first element in the database</summary>
		IReaderPair First();

		//long BytesOfStringDBOverhead();

		/// <summary>Gets the IReaderPair responsible for a given index</summary>
		IReaderPair GetByIndex(string index);

		/// <summary>Gets the multiple IReaderPairs responsible for a given index</summary>
		IReaderPair[] GetMultipleByIndex(string index);
	}

	public class Reader : IReader {
		public Reader(Stream stream, object @lock = null) {
			this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this._rawReader = new RawReader(this._stream, @lock);

			if(_stream.Length > 0) {
				//calculate overhead && first index

				//BytesOfStringDBOverhead();
				First();
			}
		}

		private Stream _stream;
		private IRawReader _rawReader;

		private IReaderPair _cacheFirstIndex = null;
		private long _overheadCache = -1; /// <inheritdoc/>

		public IReaderPair First() {
			if (this._stream.Length <= 8)
				return null;

			if (_cacheFirstIndex != null)
				return _cacheFirstIndex;

			var p = this._rawReader.ReadOn(Part.Start);

			while (!(p is IPartDataPair))
				p = this._rawReader.ReadOn(p);
				
			_cacheFirstIndex = new ReaderPair(p as IPartDataPair, this._rawReader);
			return _cacheFirstIndex;
		} /// <inheritdoc/>

		/*public long BytesOfStringDBOverhead() {
			if (_overheadCache >= 0)
				return _overheadCache;

			var p = _rawReader.ReadOn(Part.Start);

			while(p != null) {

				if (p is IPartDataPair) { //index                    | data value
					var l = (ulong)(p as IPartDataPair).ReadData(this._rawReader).Length;
					_overheadCache += sizeof(byte) + sizeof(long) +
						(l < byte.MaxValue ?
							sizeof(byte)
							: l < ushort.MaxValue ?
								sizeof(ushort)
								: l < uint.MaxValue ?
									sizeof(uint)
									: l < ulong.MaxValue ?
										sizeof(ulong)
										: throw new Exception("lolwut"));
				}

				if(p == null) {
					p = null;
				}

				p = _rawReader.ReadOn(p);
			}

			return this._overheadCache + sizeof(long);
		}*/

		public IReaderPair GetByIndex(string index) {
			if (this._stream.Length <= 8)
				return null;

			byte[] comparing = Encoding.UTF8.GetBytes(index);

			foreach (var i in this)
				if (EqualBytesLongUnrolled(comparing, i.IndexAsByteArray))
					return i;

			return null;
		} /// <inheritdoc/>

		public IReaderPair[] GetMultipleByIndex(string index) {
			if (this._stream.Length <= 8)
				return null;

			var vals = new List<IReaderPair>();
			byte[] comparing = Encoding.UTF8.GetBytes(index);

			foreach (var i in this)
				if (EqualBytesLongUnrolled(comparing, i.IndexAsByteArray))
					vals.Add(i);

			return vals.ToArray();
		} /// <inheritdoc/>

		public IEnumerator<IReaderPair> GetEnumerator() => new ReaderEnumerator(this._rawReader); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator(); /// <inheritdoc/>

		public void NotifyInsert(ICollection<KeyValuePair<string, string>> inserts) {
			foreach(var i in inserts) {
				var l = (ulong)i.Value.Length;

				_overheadCache += sizeof(byte) + sizeof(long) +
					(l < byte.MaxValue ?
						sizeof(byte)
						: l < ushort.MaxValue ?
							sizeof(ushort)
							: l < uint.MaxValue ?
								sizeof(uint)
								: l < ulong.MaxValue ?
									sizeof(ulong)
									: throw new Exception("lolwut"));
			}

			_overheadCache += sizeof(byte) + sizeof(long);
		}

		//  ___
		// /. .\

		//https://stackoverflow.com/a/33307903

		internal static unsafe bool EqualBytesLongUnrolled(byte[] data1, byte[] data2) {
			if (data1 == data2)
				return true;
			if (data1.Length != data2.Length)
				return false;

			fixed (byte* bytes1 = data1, bytes2 = data2) {
				int len = data1.Length;
				int rem = len % (sizeof(long) * 16);
				long* b1 = (long*)bytes1;
				long* b2 = (long*)bytes2;
				long* e1 = (long*)(bytes1 + len - rem);

				while (b1 < e1) {
					if (*(b1) != *(b2) || *(b1 + 1) != *(b2 + 1) ||
						*(b1 + 2) != *(b2 + 2) || *(b1 + 3) != *(b2 + 3) ||
						*(b1 + 4) != *(b2 + 4) || *(b1 + 5) != *(b2 + 5) ||
						*(b1 + 6) != *(b2 + 6) || *(b1 + 7) != *(b2 + 7) ||
						*(b1 + 8) != *(b2 + 8) || *(b1 + 9) != *(b2 + 9) ||
						*(b1 + 10) != *(b2 + 10) || *(b1 + 11) != *(b2 + 11) ||
						*(b1 + 12) != *(b2 + 12) || *(b1 + 13) != *(b2 + 13) ||
						*(b1 + 14) != *(b2 + 14) || *(b1 + 15) != *(b2 + 15))
						return false;
					b1 += 16;
					b2 += 16;
				}

				for (int i = 0; i < rem; i++)
					if (data1[len - 1 - i] != data2[len - 1 - i])
						return false;

				return true;
			}
		}
	}
}