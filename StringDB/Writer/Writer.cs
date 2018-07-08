//#define THREAD_SAFE

using StringDB.DBTypes;
using StringDB.Reader;

using System;
using System.Collections.Generic;
using System.IO;

namespace StringDB.Writer {

	/// <summary>Some kind of wwriter that writes stuff.</summary>
	public interface IWriter : IDisposable {

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(T1 index, T2 value);

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(KeyValuePair<T1, T2> kvp);

		/// <summary>Insert multiple items into the database.</summary>
		void InsertRange<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> items);

		/// <summary>Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.</summary>
		void OverwriteValue<T>(Reader.ReaderPair replacePair, T newValue);
	}

	public class Writer : IWriter {

		public Writer(Stream s, object @lock) {
			this._rawWriter = new RawWriter(s);
		}

		private IRawWriter _rawWriter;

		/// <inheritdoc/>
		public void Dispose() {
		}

		// lol if this isn't a wall of text i don't know what is

		/// <inheritdoc/>
		public void Insert<T1, T2>(T1 index, T2 value) => this.Insert(new KeyValuePair<T1, T2>(index, value)); /// <inheritdoc/>
		public void Insert<T1, T2>(KeyValuePair<T1, T2> kvp) => this.InsertRange(kvp.AsEnumerable()); /// <inheritdoc/>
		public void InsertRange<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> items) => this._rawWriter.InsertRange<T1, T2>(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), items); /// <inheritdoc/>
		public void OverwriteValue<T>(ReaderPair replacePair, T newValue) => this._rawWriter.OverwriteValue(TypeManager.GetHandlerFor<T>(), newValue, replacePair.ValueLength, replacePair._dp.DataPosition, replacePair._dp.Position + sizeof(byte));
		
		/*
			public void Insert(string index, string value) => this._rawWriter.InsertRange(WriterType<string>.StringWriterType, WriterType<string>.StringWriterType, new KeyValuePair<string, string>(index, value).AsEnumerable()); /// <inheritdoc/>
			public void Insert(KeyValuePair<string, string> kvp) => this._rawWriter.InsertRange(WriterType<string>.StringWriterType, WriterType<string>.StringWriterType, kvp.AsEnumerable()); /// <inheritdoc/>
			public void InsertRange(IEnumerable<KeyValuePair<string, string>> items) => this._rawWriter.InsertRange(WriterType<string>.StringWriterType, WriterType<string>.StringWriterType, items); /// <inheritdoc/>
			public void OverwriteValue(ReaderPair replacePair, string newValue) {
				this._rawWriter.OverwriteValue(WriterType<string>.StringWriterType, newValue, replacePair.ValueLength, replacePair._dp.DataPosition, replacePair._dp.Position + sizeof(byte));

				replacePair._byteValueCache = newValue.GetBytes();
				replacePair._strValueCache = newValue;
			}*/
	}

	/*

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

		private object _lock; /// <inheritdoc/>

		public void OverwriteValue(Reader.ReaderPair replacePair, string newValue) {
#if THREAD_SAFE
			lock (this._lock) {
#endif
			//if the value we are replacing is longer then the older value
			if (replacePair.ValueLength <= newValue.GetBytes().Length) {
				this._stream.Seek(0, SeekOrigin.End);

				var savePos = this._stream.Position;
				WriteValue(newValue);
				var raw = replacePair._dp;
				this._stream.Seek(raw.Position + 1, SeekOrigin.Begin);
				this._bw.Write(savePos);

				//when we write new data we have to update our length cache
				this._lastLength += Judge_WriteValue(newValue);
			} else { //or else, we'll just overwrite the old one
				this._stream.Seek(replacePair._dp.DataPosition, SeekOrigin.Begin);

				this.WriteValue(newValue); //the value of the new one is less then the old one
			}

			replacePair._strValueCache = newValue;
#if THREAD_SAFE
			}
#endif
		} /// <inheritdoc/>

		public void Insert(string index, string value) => InsertRange(new KeyValuePair<string, string>(index, value).AsEnumerable()); /// <inheritdoc/>

		public void Insert(KeyValuePair<string, string> kvp) => InsertRange(kvp.AsEnumerable()); /// <inheritdoc/>

		public void InsertRange(IEnumerable<KeyValuePair<string, string>> items) {
#if THREAD_SAFE
			lock (this._lock) {
#endif
			var streamLength = this._lastLength;
			var seekToPosition = streamLength < 8 ? 8 : streamLength;

			if (streamLength < 8) {
				if (streamLength != 0) //no reason to seek to 0 if we're already there
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
			}

			this._bw.Write(bytes);
		}

		private long Judge_WriteIndex(string index) {
			var bytes = index.GetBytes();

			if (bytes.Length >= Consts.MaxLength)
				throw new ArgumentException($"index.Length is longer {Consts.MaxLength}", nameof(index));

			if (bytes.Length <= 0)
				throw new ArgumentException($"index.Length is of 0 length", nameof(index));

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
	}*/
}