using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB {
	public class Reader {
		public Reader(Stream streamUse) {
			_stream = streamUse;
			_bw = new BinaryReader(_stream);
		}
		
		private Stream _stream { get; set; }
		private BinaryReader _bw { get; set; }

		public string GetValueOf(string index) => Encoding.UTF8.GetString(_GetValueOf(index));

		public string[] GetIndexes() {
			var tpl = Read(true, false);

			var data = new List<string>();

			foreach (var i in tpl.Item3)
				data.Add(Encoding.UTF8.GetString(i.Item1));

			return data.ToArray();
		}

		private byte[] _GetValueOf(string index) {
			_stream.Seek(0, SeekOrigin.Begin);

			var indxs = _GetIndexes();

			for (uint i = 0; i < indxs.Length; i++)
				if (Encoding.UTF8.GetString(indxs[i].Item1) == index) { //found it- now lets go read it
					_bw.BaseStream.Seek((long)indxs[i].Item2, SeekOrigin.Begin);
					var dataLen = _bw.ReadInt32();

					return _bw.ReadBytes(dataLen);
				}

			throw new Exception("Index doesn't exist.");
		}

		private byte[][] _GetOnlyIndexes() {
			var tpl = Read(true, false);

			var data = new List<byte[]>();

			foreach (var i in tpl.Item3)
				data.Add(i.Item1);

			return data.ToArray();
		}

		private Tuple<byte[], ulong>[] _GetIndexes() {
			var tpl = Read(true, true);

			return tpl.Item3;
		}

		public Tuple<ulong, ulong> GetIndexChain() {
			var tpl = Read(false, false);

			return Tuple.Create(tpl.Item1, tpl.Item2);
		}

		private Tuple<ulong, ulong, Tuple<byte[], ulong>[]> Read(bool storeIndexes_, bool storePositions_) {
			if (_stream.Length < 1)
				return Tuple.Create((ulong)0, (ulong)0, new Tuple<byte[], ulong>[] { Tuple.Create(new byte[0], (ulong)0) }); //throw up a bunch of """null""" data

			bool storeIndexes = true, storePositions = true;
			_stream.Seek(0, SeekOrigin.Begin);
			var data = new List<Tuple<byte[], ulong>>();

			//we will store every single piece of data unless told not to do so

			long indexChain = 0;
			long indexChainWrite = 0;

			bool inIndexes = true;

			while(inIndexes) {
				var b = _bw.ReadByte();

				if (b == Writer.IndexSeperator) { //read an index seperator - up next is the indexChain position for where to read next
					indexChainWrite = _bw.BaseStream.Position; //this will give us the write chain ( where to overwrite this index chain
					indexChain = _bw.ReadInt64(); //this will give us the location of the next indexChain

					if (indexChain == 0) //the index chain is 0 - we finished reading all of the indexes
						inIndexes = false;
					else //we haven't finished - go to the next index chain
						_bw.BaseStream.Seek((long)indexChain, SeekOrigin.Begin);
				} else {
					ulong dataPos = _bw.ReadUInt64(); //int64 is a long
					byte[] indxName = _bw.ReadBytes((int)b);

					if (storeIndexes || storePositions) {
						if (!storeIndexes)
							indxName = null;

						if (!storePositions)
							dataPos = 0;

						var tplData = Tuple.Create<byte[], ulong>(indxName, dataPos);

						data.Add(tplData);
					}
				}
			}

			return Tuple.Create(
					(ulong)indexChain,
					(ulong)indexChainWrite,
					data.ToArray()
				);
		}
	}
}
