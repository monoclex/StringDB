#define USE_UNSAFE_CODE //REMOVE THIS IF YOU DO NOT WANT TO USE UNSAFE CODE
#define USE_PINVOKE_IF_NOT_UNSAFE //REMOVE THIS IF YOU DO NOT WANT TO USE P/INVOKE AS A FALLBACK IF UNSAFE CODE IS DISABLED
//note the top: if you care to compile w/ unsafe code then you can do this

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

			FullRead(false, false, out byte[] indexSearchResult, true, Encoding.UTF8.GetBytes(index));
			return indexSearchResult;
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
			var res = FullRead(storeIndexes_, storePositions_, out var nil);
			return res;
		}

		private Tuple<ulong, ulong, Tuple<byte[], ulong>[]> FullRead(bool storeIndexes_, bool storePositions_, out byte[] indexSearch, bool searchForIndex = false, byte[] indexSearching = null) {
			indexSearch = null;

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


					if (searchForIndex) {
						if (BytesEqual(indxName, indexSearching)) {
							//var curPos = _bw.BaseStream.Position; //not sure if we'll ever need this but hey

							_bw.BaseStream.Seek((long)dataPos, SeekOrigin.Begin); //we basically only need to return this piece of data so /shrug
							indexSearch = _bw.ReadBytes(_bw.ReadInt32());
							return null; //done :)
						}
					} else if (storeIndexes || storePositions) {
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

		/*
		 * speedyness for comparing two byte array's names ~ below ~
		 * 
		 *  ___
		 * /   \
		 * 
		 *  | |
		 * 
		 */

		//keke credits over https://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net
		private static bool BytesEqual(byte[] a1, byte[] a2) {
#if USE_UNSAFE_CODE
			return EqualBytesLongUnrolled(a1, a2);
		}

		static unsafe bool EqualBytesLongUnrolled(byte[] data1, byte[] data2) {
			if (data1 == data2)
				return true;
			if (data1.Length != data2.Length)
				return false;

			fixed (byte* bytes1 = data1, bytes2 = data2) {
				int len = data1.Length;
				int rem = len % (sizeof(long) * 16);
				long* b1 = (long*)bytes1;
				long* b2 = (long*)bytes2;
				long* e1 = (long*)(bytes1 + len - rem);

				while (b1 < e1) {
					if (*(b1) != *(b2) || *(b1 + 1) != *(b2 + 1) ||
						*(b1 + 2) != *(b2 + 2) || *(b1 + 3) != *(b2 + 3) ||
						*(b1 + 4) != *(b2 + 4) || *(b1 + 5) != *(b2 + 5) ||
						*(b1 + 6) != *(b2 + 6) || *(b1 + 7) != *(b2 + 7) ||
						*(b1 + 8) != *(b2 + 8) || *(b1 + 9) != *(b2 + 9) ||
						*(b1 + 10) != *(b2 + 10) || *(b1 + 11) != *(b2 + 11) ||
						*(b1 + 12) != *(b2 + 12) || *(b1 + 13) != *(b2 + 13) ||
						*(b1 + 14) != *(b2 + 14) || *(b1 + 15) != *(b2 + 15))
						return false;
					b1 += 16;
					b2 += 16;
				}

				for (int i = 0; i < rem; i++)
					if (data1[len - 1 - i] != data2[len - 1 - i])
						return false;

				return true;
			}
		}
#elif USE_PINVOKE_IF_NOT_UNSAFE
			return ByteArrayCompare(a1, a2); //if you comment out the unsafe compare you can just use this
		}

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern int memcmp(byte[] b1, byte[] b2, long count);

		static bool ByteArrayCompare(byte[] b1, byte[] b2) {
			// Validate buffers are the same length.
			// This also ensures that the count does not exceed the length of either buffer.  
			return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
		}
#else
			return equals(a1, a2);
		}

		//what are you; ****ing gay?

		public static bool equals(byte[] a1, byte[] a2) {
			if (a1 == a2) {
				return true;
			}
			if ((a1 != null) && (a2 != null)) {
				if (a1.Length != a2.Length) {
					return false;
				}
				for (int i = 0; i < a1.Length; i++) {
					if (a1[i] != a2[i]) {
						return false;
					}
				}
				return true;
			}
			return false;
		}
#endif
	}
}
