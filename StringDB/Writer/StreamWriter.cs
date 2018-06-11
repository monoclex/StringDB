using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Writer {
	
	/// <inheritdoc/>
	public class StreamWriter : IWriter {

		/// <summary>The rewritten version of the StreamWriter</summary>
		/// <param name="s">The stream to write to</param>
		/// <param name="dbv">The database version of the stream</param>
		/// <param name="leaveOpen">If the stream should be left open</param>
		public StreamWriter(Stream s, DatabaseVersion dbv = DatabaseVersion.Latest, bool leaveOpen = false) {
			this._stream = s;
			this._dbv = DatabaseVersion.Latest;
			this._leaveOpen = leaveOpen;

			var l = new List<int>();

			l.ToArray();

#if NET20 || NET35 || NET40
			this._bw = new BinaryWriter(this._stream);
#else
			this._bw = new BinaryWriter(this._stream, Encoding.UTF8, this._leaveOpen);
#endif

			this._indexChain = 0;
			this._indexChainWrite = 0;
		}

		private Stream _stream;
		private BinaryWriter _bw;
		private DatabaseVersion _dbv;
		private bool _leaveOpen;
		private ulong _indexChain; //stores where the start of the new indexes are
		private ulong _indexChainWrite; //stores where to go to overwrite the old index chain

		/// <inheritdoc/>
		public void Insert(string index, string data) => InsertRangeGrunt(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(index, data) }); /// <inheritdoc/>
		public void Insert(string index, byte[] data) => InsertRangeGrunt(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(index, data) }); /// <inheritdoc/>
		public void Insert(string index, Stream data) => InsertRangeGrunt(new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>(index, data) }); /// <inheritdoc/>
		public void InsertRange(ICollection<KeyValuePair<string, byte[]>> data) => InsertRangeGrunt(data); /// <inheritdoc/>
		public void InsertRange(ICollection<KeyValuePair<string, string>> data) => InsertRangeGrunt(data); /// <inheritdoc/>
		public void InsertRange(ICollection<KeyValuePair<string, Stream>> data) => InsertRangeGrunt(data);

		public void Load(ulong indexChainWrite, ulong indexChain) {
			this._indexChain = indexChain;
			this._indexChainWrite = indexChainWrite;
		}

		#region nitty gritty

		#region code
		//for now this'll go unused
		//i don't want to have to deal with managing 2 versions of the writer
		private void InsertGrunt(string index, object data) {
			this._indexChain = (ulong)this._stream.Position;

			if (this._indexChainWrite != 0) {
				this._bw.BaseStream.Seek((long)this._indexChainWrite, SeekOrigin.Begin);
				this._bw.Write(this._indexChain);
				this._bw.BaseStream.Seek((long)this._indexChain, SeekOrigin.Begin);
			}

			WriteIndex(index, (ulong)this._stream.Position + Judge_WriteIndex(index) + Judge_WriteIndexSeperator());

			this._indexChainWrite = (ulong)this._stream.Position + 1uL;
			WriteIndexSeperator(0);

			WriteValue_Object(data);
		}

		private void InsertRangeGrunt(ICollection<KeyValuePair<string, byte[]>> data) {
			var l = new List<KeyValuePair<string, object>>(data.Count);

			foreach (var i in data)
				l.Add(new KeyValuePair<string, object>(i.Key, i.Value));

			InsertRangeGrunt(l);
		}

		private void InsertRangeGrunt(ICollection<KeyValuePair<string, string>> data) {
			var l = new List<KeyValuePair<string, object>>(data.Count);

			foreach (var i in data)
				l.Add(new KeyValuePair<string, object>(i.Key, i.Value));

			InsertRangeGrunt(l);
		}

		private void InsertRangeGrunt(ICollection<KeyValuePair<string, Stream>> data) {
			var l = new List<KeyValuePair<string, object>>(data.Count);

			foreach (var i in data)
				l.Add(new KeyValuePair<string, object>(i.Key, i.Value));

			InsertRangeGrunt(l);
		}

		private void InsertRangeGrunt(ICollection<KeyValuePair<string, object>> data) {
			this._indexChain = (ulong)this._stream.Position;

			if (this._indexChainWrite != 0) {
				this._bw.BaseStream.Seek((long)this._indexChainWrite, SeekOrigin.Begin);
				this._bw.Write(this._indexChain);
				this._bw.BaseStream.Seek((long)this._indexChain, SeekOrigin.Begin);
			}

			ulong totalJudgement = Judge_WriteIndexSeperator();

			foreach (var i in data)
				totalJudgement += Judge_WriteIndex(i.Key);

			foreach (var i in data) {
				WriteIndex(i.Key, (ulong)this._stream.Position + totalJudgement);

				totalJudgement += Judge_WriteValue_Object(i.Value);
				totalJudgement -= Judge_WriteIndex(i.Key);
			}

			this._indexChainWrite = (ulong)this._stream.Position + 1uL;
			WriteIndexSeperator(0);

			foreach (var i in data)
				WriteValue_Object(i.Value);
		}
		#endregion

		#region helper functions
		//index


		private void WriteIndex(string index, ulong dataPos) {
			if (index == null)
				throw new ArgumentNullException("index");

			if (index.Length >= Consts.MaxLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			this._bw.Write((byte)index.Length);
			this._bw.Write(dataPos);
			this.WriteString(index);
		}

		//seperator
		private void WriteIndexSeperator(ulong pos) {
			this._bw.Write(Consts.IndexSeperator);
			this._bw.Write(pos);
		}

		//values
		private void WriteValue_Object(object data) {
			if (data is byte[])
				WriteValue(data as byte[]);
			else if (data is string)
				WriteValue(data as string);
			else if (data is Stream)
				WriteValue(data as Stream);
			else throw new ArgumentException("data isn't string, byte[], or Stream");
		}

		private void WriteValue(byte[] data) {
			this.WriteNumber((ulong)data.Length);
			this._bw.Write(data);
		}

		private void WriteValue(string data) {
			this.WriteNumber((ulong)data.Length);
			this.WriteString(data);
		}
		
		private void WriteValue(Stream data) {
			this.WriteNumber((ulong)data.Length);

			data.Seek(0, SeekOrigin.Begin);

			int bufferLen = 4096;
			if (data.Length < bufferLen)
				bufferLen = (int)data.Length;

			int num;
			var buffer = new byte[bufferLen];
			while ((num = data.Read(buffer, 0, buffer.Length)) != 0)
				this._bw.Write(buffer);
		}
		
		//#
		private void WriteNumber(ulong val) {
			if (val <= byte.MaxValue) {
				this._bw.Write(Consts.IsByteValue);
				this._bw.Write((byte)val);
			} else if (val <= ushort.MaxValue) {
				this._bw.Write(Consts.IsUShortValue);
				this._bw.Write((ushort)val);
			} else if (val <= uint.MaxValue) {
				this._bw.Write(Consts.IsUIntValue);
				this._bw.Write((uint)val);
			} else {
				this._bw.Write(Consts.IsULongValue);
				this._bw.Write(val);
			}
		}

		private void WriteString(string data) =>
			this._bw.Write(Encoding.UTF8.GetBytes(data));

		//judging

		//index
		private ulong Judge_WriteIndex(string index) =>
			1uL + 8uL + (ulong)index.Length;

		//seperator
		private ulong Judge_WriteIndexSeperator() =>
			1uL + 8uL;

		//write value
		private ulong Judge_WriteValue_Object(object data) =>
			data is byte[] ?
				Judge_WriteValue(data as byte[])
				: data is string ?
					Judge_WriteValue(data as string)
					: data is Stream ?
						Judge_WriteValue(data as Stream)
						: throw new ArgumentException("data isn't byte[], string, or Stream");

		private ulong Judge_WriteValue(byte[] data) =>
			Judge_WriteNumber((ulong)data.Length) + (ulong)data.Length;

		private ulong Judge_WriteValue(string data) =>
			Judge_WriteNumber((ulong)data.Length) + (ulong)data.Length;

		private ulong Judge_WriteValue(Stream data) =>
			Judge_WriteNumber((ulong)data.Length) + (ulong)data.Length;

		//#
		private ulong Judge_WriteNumber(ulong val) =>
			1uL
			+ (val <= byte.MaxValue ?
				1uL
				: val <= ushort.MaxValue ?
					2uL
					: val <= uint.MaxValue ?
						4uL
						: 8uL);
		#endregion

		#endregion

		#region IDisposable Support
		/// <inheritdoc/>
		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					this._stream.Dispose();
					((IDisposable)_bw).Dispose();
				}
				
				disposedValue = true;
			}
		} /// <inheritdoc/>
			
		public void Dispose() =>
			Dispose(true);

		private bool disposedValue = false; // To detect redundant calls
#endregion
	}
}