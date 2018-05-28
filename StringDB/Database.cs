using System;
using System.Collections.Generic;
using System.IO;

namespace StringDB {
	public class Database {
		public Database(Stream output) {
			this._stream = output;

			this._reader = new Reader(this._stream);
			this._writer = new Writer(this._stream);

			if (this._stream.Length > 0) {
				var indexChain = this._reader.GetIndexChain();
				this._writer.Load(indexChain.Item1, indexChain.Item2);
			}
		}

		private Stream _stream { get; set; }
		private string _file { get; set; }

		private Reader _reader { get; set; }
		private Writer _writer { get; set; }

		public void Insert(string index, string data) => _writer.Insert(index, data);
		public void InsertRange(Dictionary<string, string> range) => _writer.InsertRange(range);
		public void InsertRange(Tuple<string, string>[] range) => _writer.InsertRange(range);

		public string[] Indexes() => _reader.GetIndexes();
		public string Get(string index) => _reader.GetValueOf(index);

		public void Flush() => _stream.Flush();
	}
}
