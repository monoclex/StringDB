using System;
using System.Collections.Generic;
using System.IO;

namespace StringDB.Writer {

	internal interface IRawWriter {

		void InsertRange<T1, T2>(WriterType<T1> wt1, WriterType<T2> wt2, IEnumerable<KeyValuePair<T1, T2>> kvps);

		void OverwriteValue<T>(WriterType<T> wt, T newValue, long oldLen, long dataPos, long posOfDataPos);
	}

	internal class RawWriter : IRawWriter {

		public RawWriter(Stream s) {
			this._s = s;
			this._bw = new BinaryWriter(s);

			this._lastStreamLength += this._s.Length;

			if (this._lastStreamLength > 8) {
				this.Seek(0);
				this._indexChainReplace = new BinaryReader(this._s).ReadInt64();
			}
		}

		private Stream _s;
		private BinaryWriter _bw;

		private long _lastStreamLength = 0;
		private long _indexChainReplace = 0;

		public void InsertRange<T1, T2>(WriterType<T1> wt1, WriterType<T2> wt2, IEnumerable<KeyValuePair<T1, T2>> kvps) {
			var pos = this._lastStreamLength;

			if (this._lastStreamLength < 8) {
				Seek(0);
				this._bw.Write(0L);
				pos = sizeof(long);
			} else Seek(this._lastStreamLength);

			var judge = pos + sizeof(byte) + sizeof(long);

			foreach (var i in kvps)
				judge += wt1.Judge(i.Key) + sizeof(byte) + sizeof(long);

			// indexes

			foreach (var i in kvps) {
				var len = wt1.Judge(i.Key);
				if (len >= Consts.MaxLength) throw new ArgumentException($"An index is longer then allowed. Length: {len}");

				this._bw.Write((byte)len);
				this._bw.Write(judge);
				wt1.Write(this._bw, i.Key);

				judge += JudgeValueLength(wt2.Judge(i.Value));
			}

			// index chain

			var repl = this._indexChainReplace;
			this._indexChainReplace = this._s.Position + sizeof(byte);
			this._bw.Write(Consts.IndexSeperator);
			this._bw.Write(0L);

			// values

			foreach (var i in kvps) {
				var len = wt2.Judge(i.Value);

				WriteValueLength(len);

				wt2.Write(this._bw, i.Value);
			}

			if (repl != 0) {
				Seek(repl);
				this._bw.Write(this._lastStreamLength);
			}

			Seek(0);
			this._bw.Write(this._indexChainReplace);

			this._lastStreamLength = judge + sizeof(byte) + sizeof(long);
		}

		public void OverwriteValue<T>(WriterType<T> wt, T newValue, long oldLen, long dataPos, long posOfDataPos) {
			var len = wt.Judge(newValue);

			if (len > oldLen) { //goto the end of the file and just slap it onto the end
				Seek(this._lastStreamLength);

				WriteValueLength(len);
				wt.Write(this._bw, newValue);

				//go to posOfDataPos and overwrite that with the lew position

				Seek(posOfDataPos);
				this._bw.Write(this._lastStreamLength);

				// update stream length

				this._lastStreamLength += JudgeValueLength(len);
			} else { // goto the data overwrite it since it's shorter
				Seek(dataPos);

				WriteValueLength(len);
				wt.Write(this._bw, newValue);
			}

			// nothin' to update
		}

		private long JudgeValueLength(long len) {
			var valueLen = len;

			if (valueLen < byte.MaxValue) valueLen += sizeof(byte);
			else if (valueLen < ushort.MaxValue) valueLen += sizeof(ushort);
			else if (valueLen < uint.MaxValue) valueLen += sizeof(uint);
			else valueLen += sizeof(ulong);

			valueLen += sizeof(byte);

			return valueLen;
		}

		private void WriteValueLength(long len) {
			if (len < byte.MaxValue) {
				this._bw.Write(Consts.IsByteValue);
				this._bw.Write((byte)len);
			} else if (len < ushort.MaxValue) {
				this._bw.Write(Consts.IsUShortValue);
				this._bw.Write((ushort)len);
			} else if (len < uint.MaxValue) {
				this._bw.Write(Consts.IsUIntValue);
				this._bw.Write((uint)len);
			} else {
				this._bw.Write(Consts.IsULongValue);
				this._bw.Write((ulong)len);
			}
		}

		private void Seek(long l) => this._s.Seek(l, SeekOrigin.Begin);

		/*
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

			this._bw.Write(bytes);
		}
		 *
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
			this._lastLength = judge;*/
	}
}