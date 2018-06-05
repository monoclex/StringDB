using StringDB.Reader;
using StringDB.Writer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StringDB {
	public class Database : IEnumerable<KeyValuePair<string, string>> {
		public Database(Stream stream, DatabaseMode dbm = DatabaseMode.ReadWrite) {
			this._stream = stream;
			this._dbm = dbm;

			if (this.Readable(dbm))
				this._reader = new Reader.StreamReader(this._stream);

			if (this.Writable(dbm))
				this._writer = new Writer.StreamWriter(this._stream);

			if (this.Readable(dbm))
				if (this._stream.Length > 0) {
					var indexChain = this._reader.GetReaderChain();

					if (!(this._writer is Writer.StreamWriter))
						throw new Exception("The Writer isn't a stream writer. This is at creator's fault - submit this as an issue to the github repo ( https://www.github.com/SirJosh3917/StringDB )");

					((Writer.StreamWriter)this._writer).Load(indexChain.IndexChainWrite, indexChain.IndexChain);
				}
		}

		private DatabaseMode _dbm { get; set; }

		private Stream _stream { get; set; }
		private string _file { get; set; }

		private IReader _reader { get; set; }
		private IWriter _writer { get; set; }

		public void Insert(string index, string data) => _writer.Insert(index, data);
		public void InsertRange(Dictionary<string, string> range) => _writer.InsertRange(range);
		public void InsertRange(Tuple<string, string>[] range) => _writer.InsertRange(range);

		public string GetValueOf(string index) => _reader.GetValueOf(index);
		public string[] GetValuesOf(string index) => _reader.GetValuesOf(index);

		public string[] Indexes() => _reader.GetIndexes();
		public string FirstIndex() => _reader.FirstIndex().Index;
		
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _reader.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _reader.GetEnumerator();

		private bool Readable(DatabaseMode e) =>
			e == DatabaseMode.ReadWrite || e == DatabaseMode.Read;

		private bool Writable(DatabaseMode e) =>
			e == DatabaseMode.ReadWrite || e == DatabaseMode.Write;
	}

	public enum DatabaseMode {
		ReadWrite = 1,
		Write = 2,
		Read = 4
	}
}
