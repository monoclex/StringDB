using System;
using System.Collections.Generic;
using System.IO;

namespace StringDB.Writer {

	internal interface IRawWriter {

		void Flush();

		void InsertRange<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, IEnumerable<KeyValuePair<T1, T2>> kvps);

		long OverwriteValue<T>(TypeHandler<T> wt, T newValue, long oldLen, long dataPos, long posOfDataPos);
	}

	internal class ThreadSafeRawWriter : IRawWriter {

		public ThreadSafeRawWriter(IRawWriter parent, object @lock) {
			this._parent = parent;
			this._lock = @lock;
		}

		private readonly IRawWriter _parent;
		private readonly object _lock;

		public void Flush() {
			lock (this._lock) this._parent.Flush();
		}

		public void InsertRange<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, IEnumerable<KeyValuePair<T1, T2>> kvps) {
			lock (this._lock) this._parent.InsertRange(wt1, wt2, kvps);
		}

		public long OverwriteValue<T>(TypeHandler<T> wt, T newValue, long oldLen, long dataPos, long posOfDataPos) {
			lock (this._lock) return this._parent.OverwriteValue(wt, newValue, oldLen, dataPos, posOfDataPos);
		}
	}

	internal class RawWriter : IRawWriter {

		public RawWriter(StreamIO s) {
			this._sio = s;
			this._lastStreamLength = s.Stream.Length;

			if (this._sio.Length > 8) {
				this._sio.Seek(0);

				// instead of using a BinaryReader, we'll just do whatever the BinaryReader does

				var m_buffer = new byte[8];

				this._sio.Stream.Read(m_buffer, 0, 4);

				// https://referencesource.microsoft.com/#mscorlib/system/io/binaryreader.cs,197

				var lo = (uint)(m_buffer[0] | m_buffer[1] << 8 |
								 m_buffer[2] << 16 | m_buffer[3] << 24);
				var hi = (uint)(m_buffer[4] | m_buffer[5] << 8 |
								 m_buffer[6] << 16 | m_buffer[7] << 24);
				this._indexChainReplace = (long)((ulong)hi) << 32 | lo;
			}
		}

		private readonly StreamIO _sio;

		private long _indexChainReplace;
		private long _lastStreamLength;

		public void Flush() => this._sio.Flush();

		public void InsertRange<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, IEnumerable<KeyValuePair<T1, T2>> kvps) {
			var pos = this._sio.Length; // get the length of the stream

			if (this._sio.Length < 8) { // handle a newly created database
				this._sio.Seek(0);
				this._sio.BinaryWriter.Write(0L);
				pos = sizeof(long);
			} else this._sio.Seek(this._sio.Length);

			var judge = pos + this._sio.WriteJumpSize(); // the position and the index chain linker lengths

			foreach (var i in kvps) // get the approximate length of every index so we know where the position of the data will be
				judge += this._sio.WriteIndexSize(wt1.GetLength(i.Key));

			// indexes

			foreach (var i in kvps) { // write the index
				var len = wt1.GetLength(i.Key);
				if (len >= Consts.MaxLength) throw new ArgumentException($"An index is longer then allowed ({Consts.MaxLength}). Length: {len}");

				this._sio.WriteIndex(wt1, len, i.Key, judge);
				
				judge += this._sio.WriteValueSize(wt2.GetLength(i.Value));
			}

			// index chain

			var repl = this._indexChainReplace; // repl for replacing the index chain at the beginning | write the index
			this._indexChainReplace = this._sio.Position;
			this._sio.WriteJump(0L);

			// values

			foreach (var i in kvps) { // write each value with the value's respective type and length
				var len = wt2.GetLength(i.Value);

				this._sio.WriteValue(wt2, len, i.Value);
			}

			if (repl != 0) { // if it's not 0 ( newly created dbs ), we'll seek to the old index chain and replace it
				this._sio.Seek(repl);
				this._sio.WriteJump(this._lastStreamLength);
			}

			this._sio.Seek(0); // seek to the beginning of the file and write the new index chain to replace
			this._sio.BinaryWriter.Write(this._indexChainReplace);

			this._lastStreamLength = this._sio.Length;
		}

		public long OverwriteValue<T>(TypeHandler<T> wt, T newValue, long oldLen, long dataPos, long posOfDataPos) {
			var len = wt.GetLength(newValue);

			if (len > oldLen) { //goto the end of the file and just slap it onto the end
				this._sio.Seek(this._sio.Length);
				var newPos = this._sio.Length;

				this._sio.WriteValue(wt, len, newValue);

				//go to posOfDataPos and overwrite that with the new position

				this._sio.Seek(posOfDataPos);
				this._sio.BinaryWriter.Write(this._sio.Length);
				
				return newPos;
			} else { // goto the data overwrite it since it's shorter
				this._sio.Seek(dataPos);

				this._sio.WriteValue(wt, len, newValue);

				return dataPos;
			}

			// nothin' to update
		}
	}
}