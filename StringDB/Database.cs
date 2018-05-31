using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StringDB {
	public class Database : IEnumerable<string> {
		public Database(Stream output, DatabaseMode dbm = DatabaseMode.ReadWrite) {
			this._stream = output;
			this._dbm = dbm;

			this._reader = new Reader(this._stream);
			this._writer = new Writer(this._stream);

			if (dbm != DatabaseMode.Read) { //if we're just reading we don't need this
				if (this._stream.Length > 0) {
					var indexChain = this._reader.GetReaderChain();
					this._writer.Load(indexChain.IndexChainWrite, indexChain.IndexChain);
				}
			}
		}

		private DatabaseMode _dbm { get; set; }

		private Stream _stream { get; set; }
		private string _file { get; set; }

		private Reader _reader { get; set; }
		private Writer _writer { get; set; }

		public void Insert(string index, string data) => _writer.Insert(index, data);
		public void InsertRange(Dictionary<string, string> range) => _writer.InsertRange(range);
		public void InsertRange(Tuple<string, string>[] range) => _writer.InsertRange(range);

		public string[] Indexes() => _reader.GetIndexes();
		public string Get(string index) => _reader.GetValueOf(index);
		public string FirstIndex() => _reader.FirstIndex().Index;
		
		public void Flush() => _stream.Flush();
		public IEnumerator<string> GetEnumerator() => _reader.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _reader.GetEnumerator();
	}

	public enum DatabaseMode {
		ReadWrite = 1,
		Write = 2,
		Read = 4
	}
}
