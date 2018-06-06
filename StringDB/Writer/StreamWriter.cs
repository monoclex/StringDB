using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Writer {
	/// <inheritdoc/>
	public class StreamWriter : IWriter {
		/// <summary>Create a new StreamWriter.</summary>
		/// <param name="outputStream">The stream to write to. You may need to call the Load() void to set the indexChain data.</param>
		public StreamWriter(Stream outputStream) {
			this._stream = outputStream;
			this._bw = new BinaryWriter(this._stream);
			this._indexChain = 0;
			this._indexChainWrite = 0;

			this._stream.Position = 0;
		}

		private Stream _stream { get; set; }
		private BinaryWriter _bw { get; set; }

		private ulong _indexChainWrite { get; set; } //stores WHERE to overwrite stuff to link TO the start of a collection of indexes.
		private ulong _indexChain { get; set; } //stores the START of a collection of indexes

		internal void Load(ulong indexChainWrite, ulong indexChain) {
			this._indexChainWrite = indexChainWrite;
			this._indexChain = indexChain;
		}

		/// <inheritdoc/>
		public void InsertRange(Dictionary<string, string> data) {
			if (data == null)
				throw new ArgumentNullException("data");

			var dat = new List<Tuple<string, string>>();

			foreach (var i in data)
				dat.Add(Tuple.Create(i.Key, i.Value));

			this.InsertRange(dat.ToArray());
		}

		/// <inheritdoc/>
		public void Insert(string index, string data) => InsertRange(new Tuple<string, string>[] { Tuple.Create(index, data) });

		/// <inheritdoc/>
		public void InsertRange(Tuple<string, string>[] data) {
			if (data == null)
				throw new ArgumentNullException("data");

			this._bw.Seek(0, SeekOrigin.End); //goto the end of the stream to append data

			var indxChain = (ulong)0; //responsible for storing the start of the index
			var indxChainWrite = (ulong)0; //responsible for storing the location of where to overwrite the indexChain

			var indx = new string[data.Length];
			var dta = new string[data.Length];

			var indxsAt = new Dictionary<string, ulong>();
			var indxsShowsUpAt = new Dictionary<string, ulong>();

			//SET DATA
			for (uint i = 0; i < data.Length; i++) {
				indx[i] = data[i].Item1;
				dta[i] = data[i].Item2;
			}

			//SET INDEX CHAIN
			indxChain = (ulong)this._stream.Position;

			//WRITE EACH INDEX
			for (uint i = 0; i < indx.Length; i++) {
				if (indx[i].Length > 254)
					throw new Exception($"Index cannot be longer then 254 chars. 0xFF is reserved for the index chain. {indx[i]}");

				this._bw.Write(Convert.ToByte(indx[i].Length));  //LENGTH OF INDEXER
				indxsAt[indx[i]] = (ulong)this._stream.Position;
				this._bw.Write((ulong)0);                //WHERE IT WILL SHOW UP IN THE FILE

				WriteStringRaw(indx[i]);
			}

			//SEPERATE INDEXES
			this._bw.Write(Consts.IndexSeperator);

			indxChainWrite = (ulong)this._stream.Position;
			this._bw.Write((ulong)0); //if we want to append more data, we'll link this to the next set of indexes. aka the index chain

			//WRITE DATA
			for (uint i = 0; i < dta.Length; i++) {
				indxsShowsUpAt[indx[i]] = (ulong)this._stream.Position; //we wanna know when we'll see it again

				this._bw.Write(dta[i].Length);		//LENTH OF DATA
				WriteStringRaw(dta[i]);			//THE DATA ITSELF
			}
			
			//REWRITE INDEXES TO POINT TO THE DATA
			foreach(var i in indxsAt) {
				var overwriteAt = i.Value;
				var locationShowsUp = indxsShowsUpAt[i.Key];

				//overwrite this
				this._bw.BaseStream.Seek((long)overwriteAt, SeekOrigin.Begin);
				this._bw.Write(locationShowsUp); //overwrite the old data
			}

			//INDEX CHAIN THIS NEW DATA WITH THE OLD INDEXES
			if(this._indexChainWrite != 0) { //if the place to overwrite the old index chain exists
				this._bw.BaseStream.Seek((long)this._indexChainWrite, SeekOrigin.Begin); //go to it
				this._bw.Write(indxChain); //write the start of the new index chain
			}

			//SET NEW INDEX CHAIN
			this._indexChain = indxChain;
			this._indexChainWrite = indxChainWrite;

			//DONE
		}

		private void WriteStringRaw(string raw) { if (raw == null) throw new ArgumentNullException("an index within the data"); foreach (var i in raw) this._bw.Write(i); }
	}
}