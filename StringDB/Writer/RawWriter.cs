using StringDB.DBTypes;

using System;
using System.Collections.Generic;
using System.IO;

namespace StringDB.Writer {

	internal interface IRawWriter {

		void Flush();

		void InsertRange<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, IEnumerable<KeyValuePair<T1, T2>> kvps);

		long OverwriteValue<T>(TypeHandler<T> wt, T newValue, long oldLen, long dataPos, long posOfDataPos);
	}

	//TODO: remove IRawWriter

	internal class RawWriter : IRawWriter {

		public RawWriter(Stream s) {
			this._s = s;
			this._bw = new BinaryWriter(s);

			this._lastStreamLength += this._s.Length;

			if (this._lastStreamLength > 8) {
				this._s.Seek(0);

				// instead of using a BinaryReader, we'll just do whatever the BinaryReader does

				var m_buffer = new byte[8];

				this._s.Read(m_buffer, 0, 8);

				// https://referencesource.microsoft.com/#mscorlib/system/io/binaryreader.cs,197

				var lo = (uint)(m_buffer[0] | m_buffer[1] << 8 |
								 m_buffer[2] << 16 | m_buffer[3] << 24);
				var hi = (uint)(m_buffer[4] | m_buffer[5] << 8 |
								 m_buffer[6] << 16 | m_buffer[7] << 24);
				this._indexChainReplace = (long)((ulong)hi) << 32 | lo;
			}
		}

		private readonly Stream _s;
		private readonly BinaryWriter _bw;

		private long _lastStreamLength;
		private long _indexChainReplace;

		public void Flush() => this._bw.Flush();

		public void InsertRange<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, IEnumerable<KeyValuePair<T1, T2>> kvps) {
			var pos = this._lastStreamLength; // get the length of the stream

			if (this._lastStreamLength < 8) { // handle a newly created database
				this._s.Seek(0);
				this._bw.Write(0L);
				pos = sizeof(long);
			} else this._s.Seek(this._lastStreamLength);

			var judge = pos + sizeof(byte) + sizeof(long); // the position and the index chain linker lengths

			foreach (var i in kvps) // get the approximate length of every index so we know where the position of the data will be
				judge += wt1.GetLength(i.Key) + sizeof(byte) + sizeof(long);

			// indexes

			foreach (var i in kvps) { // write the index
				var len = wt1.GetLength(i.Key);
				if (len >= Consts.MaxLength) throw new ArgumentException($"An index is longer then allowed ({Consts.MaxLength}). Length: {len}");

				this._bw.Write((byte)len);
				this._bw.Write(judge);
				wt1.Write(this._bw, i.Key);

				var wlen = wt2.GetLength(i.Value); // judge the next value pos
				judge += TypeHandlerLengthManager.EstimateWriteLengthSize(wlen) + wlen;
			}

			// index chain

			var repl = this._indexChainReplace; // repl for replacing the index chain at the beginning | write the index
			this._indexChainReplace = this._s.Position + sizeof(byte);
			this._bw.Write(Consts.IndexSeperator);
			this._bw.Write(0L);

			// values

			foreach (var i in kvps) { // write each value with the value's respective type and length
				var len = wt2.GetLength(i.Value);

				this._bw.Write(wt2.Id);
				TypeHandlerLengthManager.WriteLength(this._bw, wt2.GetLength(i.Value));
				wt2.Write(this._bw, i.Value);
			}

			if (repl != 0) { // if it's not 0 ( newly created dbs ), we'll seek to the old index chain and replace it
				this._s.Seek(repl);
				this._bw.Write(this._lastStreamLength);
			}

			this._s.Seek(0); // seek to the beginning of the file and write the new index chain to replace
			this._bw.Write(this._indexChainReplace);

			this._lastStreamLength = judge + sizeof(byte) + sizeof(long); // set the new length of the file
		}

		public long OverwriteValue<T>(TypeHandler<T> wt, T newValue, long oldLen, long dataPos, long posOfDataPos) {
			var len = wt.GetLength(newValue);

			if (len > oldLen) { //goto the end of the file and just slap it onto the end
				this._s.Seek(this._lastStreamLength);
				var newPos = this._lastStreamLength;

				this._bw.Write(wt.Id);
				TypeHandlerLengthManager.WriteLength(this._bw, len);
				wt.Write(this._bw, newValue);

				//go to posOfDataPos and overwrite that with the new position

				this._s.Seek(posOfDataPos);
				this._bw.Write(this._lastStreamLength);

				// update stream length

				this._lastStreamLength += TypeHandlerLengthManager.EstimateWriteLengthSize(len) + len + sizeof(byte);

				return newPos;
			} else { // goto the data overwrite it since it's shorter
				this._s.Seek(dataPos);

				this._bw.Write(wt.Id);
				TypeHandlerLengthManager.WriteLength(this._bw, len);
				wt.Write(this._bw, newValue);

				return dataPos;
			}

			// nothin' to update
		}
	}
}