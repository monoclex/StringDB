//#define THREAD_SAFE

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Writer {
	public interface IWriter : IDisposable {
		/// <summary>Insert an item into the database</summary>
		void Insert(string index, string value);

		/// <summary>Insert multiple items into the database</summary>
		void InsertRange(ICollection<KeyValuePair<string, string>> items);
	}

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
		}

		private Stream _stream;
		private BinaryWriter _bw;

		internal long _indexChainReplace = 0;

		private long _lastPos = -1;

		private object _lock = null; /// <inheritdoc/>

		public void Insert(string index, string value) => InsertRange(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(index, value) }); /// <inheritdoc/>
		public void InsertRange(ICollection<KeyValuePair<string, string>> items) {
#if THREAD_SAFE
			lock (this._lock) {
#endif
			var l = this._stream.Length;

				if (l >= 8)
					this._stream.Seek(0, SeekOrigin.End);
				else {
					_Seek(0);
					this._bw.Write((long)0);
					_Seek(8);
				}

				//replace index chain
				var p = l < 8 ? 8 : l;

				_Seek(_indexChainReplace);
				this._bw.Write(p);
				_Seek(p);

				var judge = p + sizeof(byte) + sizeof(long);

				//judge the amount of space it'd require to write each item
				foreach (var i in items)
					judge += Judge_WriteIndex(i.Key);

				//foreach item
				foreach (var i in items) {
					WriteIndex(i.Key, judge);

					//judge -= Judge_WriteIndex(i.Key);
					judge += Judge_WriteValue(i.Value);
				}

				this._bw.Write((byte)0xFF);
				this._indexChainReplace = this._stream.Position;
				this._bw.Write((long)0);

				foreach (var i in items) {
					WriteValue(i.Value);
				}

				_Seek(0);
				this._bw.Write((long)this._indexChainReplace);
#if THREAD_SAFE
			}
#endif
		}

		public void Dispose() {
			this._bw.Flush();
			this._stream.Flush();

			this._bw = null;
			this._stream = null;
		}

		private void WriteIndex(string index, long nextPos) {
			this._bw.Write((byte)index.Length);
			this._bw.Write(nextPos);
			this._bw.Write(Encoding.UTF8.GetBytes(index));
		}

		private void WriteValue(string value) {
			if (value.Length <= byte.MaxValue) {
				this._bw.Write(Consts.IsByteValue);
				this._bw.Write((byte)value.Length);
			} else if (value.Length <= ushort.MaxValue) {
				this._bw.Write(Consts.IsUShortValue);
				this._bw.Write((ushort)value.Length);
			} else if ((ulong)value.Length <= uint.MaxValue) {
				this._bw.Write(Consts.IsUIntValue);
				this._bw.Write((uint)value.Length);
			} else if ((ulong)value.Length <= ulong.MaxValue) {
				this._bw.Write(Consts.IsULongValue);
				this._bw.Write((ulong)value.Length);
			} else throw new Exception("lolwut");

			this._bw.Write(Encoding.UTF8.GetBytes(value));
		}


		private long Judge_WriteIndex(string index) =>
			sizeof(byte) + sizeof(long) + (long)index.Length;

		private long Judge_WriteValue(string value) =>
			sizeof(byte) +
			(long)value.Length +
			(value.Length <= byte.MaxValue ?
				sizeof(byte)
				: value.Length <= ushort.MaxValue ?
					sizeof(ushort)
					: (ulong)value.Length <= uint.MaxValue ?
						sizeof(uint)
						: (ulong)value.Length <= ulong.MaxValue ?
							sizeof(ulong)
							: throw new Exception("lolwut"));

		private void _Seek(long pos) {
			this._stream.Seek(pos, SeekOrigin.Begin);
		}
	}
}