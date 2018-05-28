using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {
	public class Writer {
		public Writer(Stream outputStream) {
			_stream = outputStream;
			_bw = new BinaryWriter(_stream);
			_indexChain = 0;
			_indexChainWrite = 0;

			_stream.Position = 0;
		}

		public const byte IndexSeperator = 0xFF;

		private Stream _stream { get; set; }
		private BinaryWriter _bw { get; set; }

		private ulong _indexChainWrite { get; set; } //stores WHERE to overwrite stuff to link TO the start of a collection of indexes.
		private ulong _indexChain { get; set; } //stores the START of a collection of indexes

		public void Load(ulong indexChainWrite, ulong indexChain) {
			this._indexChainWrite = indexChainWrite;
			this._indexChain = indexChain;
		}

		public void InsertRange(Dictionary<string, string> data) {
			var dat = new List<Tuple<string, string>>();

			foreach (var i in data)
				dat.Add(Tuple.Create(i.Key, i.Value));

			this.InsertRange(dat.ToArray());
		}

		public void Insert(string index, string data) => InsertRange(new Tuple<string, string>[] { Tuple.Create(index, data) });

		public void InsertRange(Tuple<string, string>[] data) {
			_bw.Seek(0, SeekOrigin.End); //goto the end of the stream to append data

			var indxChain = (ulong)0; //responsible for storing the start of the index
			var indxChainWrite = (ulong)0; //responsible for storing the location of where to overwrite the indexChain

			string[] indx = new string[data.Length];
			string[] dta = new string[data.Length];

			var indxsAt = new Dictionary<string, ulong>();
			var indxsShowsUpAt = new Dictionary<string, ulong>();

			//SET DATA
			for (uint i = 0; i < data.Length; i++) {
				indx[i] = data[i].Item1;
				dta[i] = data[i].Item2;
			}

			//SET INDEX CHAIN
			indxChain = (ulong)_stream.Position;

			//WRITE EACH INDEX
			for (uint i = 0; i < indx.Length; i++) {
				if (indx[i].Length > 253)
					throw new Exception($"Index cannot be longer then 253 chars. {indx[i]}");

				_bw.Write(Convert.ToByte(indx[i].Length));  //LENGTH OF INDEXER
				indxsAt[indx[i]] = (ulong)_stream.Position;
				_bw.Write((ulong)0);                //WHERE IT WILL SHOW UP IN THE FILE

				WriteStringRaw(indx[i]);
			}

			//SEPERATE INDEXES
			_bw.Write(IndexSeperator);

			indxChainWrite = (ulong)_stream.Position;
			_bw.Write((ulong)0); //if we want to append more data, we'll link this to the next set of indexes. aka the index chain

			//WRITE DATA
			for (uint i = 0; i < dta.Length; i++) {
				indxsShowsUpAt[indx[i]] = (ulong)_stream.Position; //we wanna know when we'll see it again

				_bw.Write(dta[i].Length);		//LENTH OF DATA
				WriteStringRaw(dta[i]);			//THE DATA ITSELF
			}
			
			//REWRITE INDEXES TO POINT TO THE DATA
			foreach(var i in indxsAt) {
				var overwriteAt = i.Value;
				var locationShowsUp = indxsShowsUpAt[i.Key];

				//overwrite this
				_bw.Seek((int)overwriteAt, SeekOrigin.Begin);
				_bw.Write(locationShowsUp); //overwrite the old data
			}

			//INDEX CHAIN THIS NEW DATA WITH THE OLD INDEXES
			if(_indexChainWrite != 0) { //if the place to overwrite the old index chain exists
				_bw.Seek((int)_indexChainWrite, SeekOrigin.Begin); //go to it
				_bw.Write(indxChain); //write the start of the new index chain
			}

			//SET NEW INDEX CHAIN
			_indexChain = indxChain;
			_indexChainWrite = indxChainWrite;

			//DONE
		}

		private void WriteStringRaw(string raw) { foreach (var i in raw) _bw.Write(i); }
	}
}