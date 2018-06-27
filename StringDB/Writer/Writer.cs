//#define THREAD_SAFE

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Writer {
	/// <summary>Some kind of wwriter that writes stuff.</summary>
	public interface IWriter : IDisposable {
		/// <summary>Insert an item into the database</summary>
		void Insert(string index, string value);

		/// <summary>Insert an item into the database</summary>
		void Insert(KeyValuePair<string, string> kvp);

		/// <summary>Insert multiple items into the database.</summary>
		/// <remarks>DO NOT FEED A yield return; ON THIS - if you feed in an IEnumerable that is generated via yield return, please make sure that the yield returns are constant. Make sure NO RNG AT ALL is used, or StringDB will fail at writing your stuff. This is because it iterates over the IEnumerable 3 times. If you'd like to use RNG, call a .ToList() ( System.Linq ) on it so it stays constant and everybody has a happy day.</remarks>
		void InsertRange(IEnumerable<KeyValuePair<string, string>> items);

		/// <summary>Overwrite a value. Note: The old value is still left in the file, and a database cleanup function is needed to be implemented soon.</summary>
		void OverwriteValue(Reader.ReaderPair replacePair, string newValue);
	}

	/// <inheritdoc/>
	public class Writer : IWriter {
		internal Writer(Stream s, object @lock = null) {
			this._stream = s;
			this._bw = new BinaryWriter(this._stream);
			this._lock = @lock;

			if (this._lock == null)
				this._lock = new object();

			if (s.Length > 0) {
				var br = new BinaryReader(this._stream);

				_Seek(0);
				this._indexChainReplace = br.ReadInt64();
			}

			this._lastLength = s.Length;
			_Seek(0); //seek to the beginning
		}

		private Stream _stream;
		private BinaryWriter _bw;

		internal long _indexChainReplace = 0;

		private long _lastLength = 0;
		private long _lastPos = -1;

		private object _lock; /// <inheritdoc/>

		public void OverwriteValue(Reader.ReaderPair replacePair, string newValue) {
#if THREAD_SAFE
			lock (this._lock) {
#endif
			//if the value we are replacing is longer then the older value
			if (replacePair.Value.GetBytes().Length > newValue.GetBytes().Length) {

				this._stream.Seek(0, SeekOrigin.End);

				var savePos = this._stream.Position;
				WriteValue(newValue);
				var raw = replacePair._dp;
				this._stream.Seek(raw.Position + 1, SeekOrigin.Begin);
				this._bw.Write(savePos);

				replacePair._valueCache = newValue;

			} else { //or else, we'll just overwrite the old one
				this._stream.Seek(replacePair._dp.DataPosition, SeekOrigin.Begin);

				this.WriteValue(newValue); //the value of the new one is less then the old one
			}
#if THREAD_SAFE
			}
#endif
		} /// <inheritdoc/>

		public void Insert(string index, string value) => InsertRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(index, value) }); /// <inheritdoc/>
		public void Insert(KeyValuePair<string, string> kvp) => InsertRange(new KeyValuePair<string, string>[] { kvp }); /// <inheritdoc/>
		public void InsertRange(IEnumerable<KeyValuePair<string, string>> items) {
#if THREAD_SAFE
			lock (this._lock) {
#endif
			var streamLength = this._lastLength;
			var seekToPosition = streamLength < 8 ? 8 : streamLength;

			if (streamLength < 8) {
				if(streamLength != 0) //no reason to seek to 0 if we're already there
					_Seek(0);

				this._bw.Write(0L);

				seekToPosition = 8;
			}
			
			_Seek(seekToPosition);

			//current pos + index chain
			var judge = seekToPosition + sizeof(byte) + sizeof(long);

			foreach (var i in items)
				judge += Judge_WriteIndex(i.Key);

			//foreach item
			foreach (var i in items) {
				WriteIndex(i.Key, judge);

				//judge -= Judge_WriteIndex(i.Key);
				judge += Judge_WriteValue(i.Value);
			}

			var oldRepl = this._indexChainReplace; //where we need to go to replace the old

			this._bw.Write(Consts.IndexSeperator);
			this._indexChainReplace = this._stream.Position;
			this._bw.Write(0L);

			foreach (var i in items) {
				WriteValue(i.Value);
			}

			if (oldRepl != 0) { //we'll be seeking and rewriting, so we want to make sure we don't do that
				_Seek(oldRepl); //seek here to oldRepl
				this._bw.Write(seekToPosition);
			}
			
			_Seek(0); //and then seek to 0 and replace that
			this._bw.Write(this._indexChainReplace);

			//set the last length to what we judged would be the future
			//because what we judged is exactly the amount we re-wrote over
			this._lastLength = judge;
#if THREAD_SAFE
			}
#endif
		} /// <inheritdoc/>

		public void Dispose() {
			this._bw.Flush();
			this._stream.Flush();

			this._bw = null;
			this._stream = null;
		}

		private void WriteIndex(string index, long nextPos) {
			var bytes = index.GetBytes();

			this._bw.Write((byte)bytes.Length); //we know it's less then 255 so we're safe
			this._bw.Write(nextPos);
			this._bw.Write(bytes);
		}

		private void WriteValue(string value) {
			var bytes = value.GetBytes();

			if (bytes.Length <= byte.MaxValue) {
				this._bw.Write(Consts.IsByteValue);
				this._bw.Write((byte)bytes.Length);
			} else if (bytes.Length <= ushort.MaxValue) {
				this._bw.Write(Consts.IsUShortValue);
				this._bw.Write((ushort)bytes.Length);
			} else {
				this._bw.Write(Consts.IsUIntValue);
				this._bw.Write(bytes.Length);
			}
			
			/*else if ((ulong)bytes.Length <= uint.MaxValue) {
				this._bw.Write(Consts.IsUIntValue);
				this._bw.Write((uint)bytes.Length);
			} else if ((ulong)bytes.Length <= ulong.MaxValue) {
				this._bw.Write(Consts.IsULongValue);
				this._bw.Write((ulong)bytes.Length);
			}*/

			this._bw.Write(bytes);
		}
		
		private long Judge_WriteIndex(string index) {
			var bytes = index.GetBytes();

			if (bytes.Length >= Consts.MaxLength)
				throw new ArgumentException($"index.Length is longer {Consts.MaxLength}", nameof(index));

			if (bytes.Length == 0)
				throw new ArgumentException($"index.Length is of 0 lenth", nameof(index));

			return sizeof(byte) + sizeof(long) + bytes.Length;
		}

		private long Judge_WriteValue(string value) =>
			Judge_WriteValue(value.GetBytes());

		private long Judge_WriteValue(byte[] value) =>
			sizeof(byte) +
			(value.Length <= byte.MaxValue ?
				sizeof(byte)
				: value.Length <= ushort.MaxValue ?
					sizeof(ushort)
					: sizeof(int)) +
			value.Length;

		private void _Seek(long pos) => this._stream.Seek(pos, SeekOrigin.Begin);
	}
}