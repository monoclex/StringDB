using System;
using System.Collections.Generic;
using System.IO;

namespace StringDB {
	public class Database {
		public Database(Stream output, DatabaseMode dbm = DatabaseMode.ReadWrite) {
			this._stream = output;
			this._dbm = dbm;

			this._reader = new Reader(this._stream);
			this._writer = new Writer(this._stream);

			if (dbm != DatabaseMode.Read) { //if we're just reading we don't need this
				if (this._stream.Length > 0) {
					var indexChain = this._reader.GetIndexChain();
					this._writer.Load(indexChain.Item1, indexChain.Item2);
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
		public string FirstIndex() => _reader.FirstIndex();

		public ulong lastAsk = 0;
		public string IndexAfter(string index) {
			var res = _reader.NextIndex(index, lastAsk);
			if (res == null)
				throw new Exception("End of stream."); //note: this never happens

			this.lastAsk = res.Item2;
			return res.Item1;
		}

		public void Flush() => _stream.Flush();
	}

	public enum DatabaseMode {
		ReadWrite = 1,
		Write = 2,
		Read = 4
	}
}
