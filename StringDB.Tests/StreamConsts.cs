using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace StringDB.Tests {
	public class StreamData<T> {
		private static byte[] GetBytesOf(string s) => Encoding.UTF8.GetBytes(s);
		internal static Stream GetStreamOf(string data) {
			var s = new MemoryStream();
			var sw = new BinaryWriter(s, Encoding.UTF8, true);

			sw.Write(GetBytesOf(data));

			return s;
		}

		#region data consts
		#region string
		public static readonly StreamData<object> OnePerOne_String = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", "TestValue1")
				}
			);

		public static readonly StreamData<object> OnePerTwo_String = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", "TestValue1")
				},
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test2", "TestValue2")
				}
			);

		public static readonly StreamData<object> TwoPerOne_String = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", "TestValue1"),
					new KeyValuePair<string, object>("Test2", "TestValue2")
				}
			);

		public static readonly StreamData<object> TwoPerTwo_String = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", "TestValue1"),
					new KeyValuePair<string, object>("Test2", "TestValue2")
				},
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test3", "TestValue3"),
					new KeyValuePair<string, object>("Test4", "TestValue4")
				}
			);
		#endregion

		#region byte
		public static readonly StreamData<object> OnePerOne_Bytes = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetBytesOf("TestValue1"))
				}
			);

		public static readonly StreamData<object> OnePerTwo_Bytes = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetBytesOf("TestValue1"))
				},
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test2", GetBytesOf("TestValue2"))
				}
			);

		public static readonly StreamData<object> TwoPerOne_Bytes = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetBytesOf("TestValue1")),
					new KeyValuePair<string, object>("Test2", GetBytesOf("TestValue2"))
				}
			);

		public static readonly StreamData<object> TwoPerTwo_Bytes = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetBytesOf("TestValue1")),
					new KeyValuePair<string, object>("Test2", GetBytesOf("TestValue2"))
				},
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test3", GetBytesOf("TestValue3")),
					new KeyValuePair<string, object>("Test4", GetBytesOf("TestValue4"))
				}
			);
		#endregion

		#region stream
		public static readonly StreamData<object> OnePerOne_Stream = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetStreamOf("TestValue1"))
				}
			);

		public static readonly StreamData<object> OnePerTwo_Stream = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetStreamOf("TestValue1"))
				},
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test2", GetStreamOf("TestValue2"))
				}
			);

		public static readonly StreamData<object> TwoPerOne_Stream = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetStreamOf("TestValue1")),
					new KeyValuePair<string, object>("Test2", GetStreamOf("TestValue2"))
				}
			);

		public static readonly StreamData<object> TwoPerTwo_Stream = new StreamData<object>(
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test1", GetStreamOf("TestValue1")),
					new KeyValuePair<string, object>("Test2", GetStreamOf("TestValue2"))
				},
				new List<KeyValuePair<string, object>>() {
					new KeyValuePair<string, object>("Test3", GetStreamOf("TestValue3")),
					new KeyValuePair<string, object>("Test4", GetStreamOf("TestValue4"))
				}
			);
		#endregion
		#endregion

		public StreamData(params List<KeyValuePair<string, T>>[] defaultValues) {
			this.Data = new List<List<KeyValuePair<string, T>>>();
			this.Indexes = new string[0];
			this.Values = new T[0];

			foreach (var j in defaultValues)
				AddData(j.ToArray());
		}

		public void AddData(KeyValuePair<string, T> piece) {
			AddKVP(piece);

			this.Data.Add(new List<KeyValuePair<string, T>>() { piece });
		}

		public void AddData(KeyValuePair<string, T>[] pieces) {
			foreach (var i in pieces)
				AddKVP(i);

			this.Data.Add(pieces.ToList());
		}

		private void AddKVP(KeyValuePair<string, T> p) {
			AddIndex(p.Key);
			AddValue(p.Value);
		}

		private void AddIndex(string i) {
			var tmpInd = new string[this.Indexes.Length + 1];
			this.Indexes.CopyTo(tmpInd, 0);
			tmpInd[this.Indexes.Length] = i;
			this.Indexes = tmpInd;
		}

		private void AddValue(T i) {
			var tmpDat = new T[this.Values.Length + 1];
			this.Values.CopyTo(tmpDat, 0);
			tmpDat[this.Values.Length] = i;
			this.Values = tmpDat;
		}

		public List<List<KeyValuePair<string, T>>> Data { get; private set; }
		public string[] Indexes { get; set; }
		public T[] Values { get; set; }

		//todo: not awful
		public List<List<KeyValuePair<string, string>>> Data_String() {
			var res = new List<List<KeyValuePair<string, string>>>();

			foreach (var i in this.Data) {
				var l = new List<KeyValuePair<string, string>>();

				foreach (var j in i) {
					if (!(j.Value is string))
						return null;

					l.Add(new KeyValuePair<string, string>(j.Key, j.Value as string));
				}

				res.Add(l);
			}

			return res;
		}

		public List<List<KeyValuePair<string, byte[]>>> Data_Bytes() {
			var res = new List<List<KeyValuePair<string, byte[]>>>();

			foreach (var i in this.Data) {
				var l = new List<KeyValuePair<string, byte[]>>();

				foreach (var j in i) {
					if (!(j.Value is string))
						return null;

					l.Add(new KeyValuePair<string, byte[]>(j.Key, j.Value as byte[]));
				}

				res.Add(l);
			}

			return res;
		}

		public List<List<KeyValuePair<string, Stream>>> Data_Stream() {
			var res = new List<List<KeyValuePair<string, Stream>>>();

			foreach (var i in this.Data) {
				var l = new List<KeyValuePair<string, Stream>>();

				foreach (var j in i) {
					if (!(j.Value is string))
						return null;

					l.Add(new KeyValuePair<string, Stream>(j.Key, j.Value as Stream));
				}

				res.Add(l);
			}

			return res;
		}
	}

	public static class StreamConsts {
		public static Stream BlankStream() =>
			new MemoryStream();

		private static ulong CalculateOverhead(long total, StreamData<object> dat) {
			var res = (ulong)total;

			foreach(var i in dat.Data) {
				foreach(var j in i) {
					res -= (ulong)j.Key.Length;
					res -= Judge_WriteIndex(j.Value) - Judge_WriteIndex_ConstBegin;
				}
			}

			return res;
		}

		public enum Type {
			String = 0,
			Bytes = 1,
			Stream = 2
		}

		public static GeneratedStream GetBy(int something, int persomething, Type type) {
			if (something == 1) {
				if (persomething == 1)
					switch (type) {
						case Type.String:
						return OnePerOne(StreamData<object>.OnePerOne_String);
						case Type.Bytes:
						return OnePerOne(StreamData<object>.OnePerOne_Bytes);
						case Type.Stream:
						return OnePerOne(StreamData<object>.OnePerOne_Stream);
					} else if (persomething == 2)
					switch (type) {
						case Type.String:
						return OnePerTwo(StreamData<object>.OnePerTwo_String);
						case Type.Bytes:
						return OnePerTwo(StreamData<object>.OnePerTwo_Bytes);
						case Type.Stream:
						return OnePerTwo(StreamData<object>.OnePerTwo_Stream);
					}
			} else if (something == 2) {
				if (persomething == 1)
					switch (type) {
						case Type.String:
						return TwoPerOne(StreamData<object>.TwoPerOne_String);
						case Type.Bytes:
						return TwoPerOne(StreamData<object>.TwoPerOne_Bytes);
						case Type.Stream:
						return TwoPerOne(StreamData<object>.TwoPerOne_Stream);
					} else if (persomething == 2)
					switch (type) {
						case Type.String:
						return TwoPerTwo(StreamData<object>.TwoPerTwo_String);
						case Type.Bytes:
						return TwoPerTwo(StreamData<object>.TwoPerTwo_Bytes);
						case Type.Stream:
						return TwoPerTwo(StreamData<object>.TwoPerTwo_Stream);
					}
			}

			return null;
		}

		public static GeneratedStream OnePerOne(StreamData<object> dat) {
			//_one_ value per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s,
				StreamDat = dat
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(dat.Indexes[0], (ulong)s.Position + Judge_WriteIndexSeperator + Judge_WriteIndex(dat.Indexes[0]), sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(dat.Values[0], sw, gs);
			sw.Flush();

			gs.Overhead = CalculateOverhead(s.Length, dat);

			return gs;

		}

		public static GeneratedStream TwoPerOne(StreamData<object> dat) {
			//_two_ values per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s,
				StreamDat = dat
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(dat.Indexes[0], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[0]) + Judge_WriteIndex(dat.Indexes[1]) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndex(dat.Indexes[1], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[1]) + Judge_WriteIndexSeperator + Judge_WriteValue(dat.Values[0]), sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(dat.Values[0], sw, gs);
			WriteValue(dat.Values[1], sw, gs);
			sw.Flush();

			gs.Overhead = CalculateOverhead(s.Length, dat);

			return gs;
		}

		public static GeneratedStream OnePerTwo(StreamData<object> dat) {
			//_two_ values per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s,
				StreamDat = dat
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(dat.Indexes[0], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[0]) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndexSeperator((ulong)s.Position + Judge_WriteIndexSeperator + Judge_WriteValue(dat.Values[0]), sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(dat.Values[0], sw, gs);
			WriteIndex(dat.Indexes[1], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[1]) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndexSeperator(0, sw);
			WriteValue(dat.Values[1], sw, gs);
			sw.Flush();

			gs.Overhead = CalculateOverhead(s.Length, dat);

			return gs;
		}

		public static GeneratedStream TwoPerTwo(StreamData<object> dat) {
			//_two_ values per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s,
				StreamDat = dat
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(dat.Indexes[0], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[0]) + Judge_WriteIndex(dat.Indexes[1]) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndex(dat.Indexes[1], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[1]) + Judge_WriteIndexSeperator + Judge_WriteValue(dat.Values[0]), sw, gs);
			WriteIndexSeperator((ulong)s.Position + Judge_WriteIndexSeperator + Judge_WriteValue(dat.Values[0]) + Judge_WriteValue(dat.Values[1]), sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(dat.Values[0], sw, gs);
			WriteValue(dat.Values[1], sw, gs);
			WriteIndex(dat.Indexes[2], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[2]) + Judge_WriteIndex(dat.Indexes[3]) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndex(dat.Indexes[3], (ulong)s.Position + Judge_WriteIndex(dat.Indexes[3]) + Judge_WriteIndexSeperator + Judge_WriteValue(dat.Values[2]), sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(dat.Values[2], sw, gs);
			WriteValue(dat.Values[3], sw, gs);
			sw.Flush();

			gs.Overhead = CalculateOverhead(s.Length, dat);

			return gs;
		}

		private static void WriteIndex(string indexName, ulong dataPos, BinaryWriter sw, GeneratedStream gs) {
			sw.Write((byte)(indexName.Length));
			sw.Write((ulong)dataPos);
			sw.Write(Encoding.UTF8.GetBytes(indexName));

			gs.AddIndex(indexName, (ulong)sw.BaseStream.Position - (ulong)indexName.Length - 8uL - 1uL);
		}

		private static ulong Judge_WriteIndex(object indexName) =>
			indexName is string ?
				Judge_WriteIndex_((string)indexName)
				: indexName is byte[] ?
					Judge_WriteIndex_((byte[])indexName)
					: Judge_WriteIndex_((Stream)indexName);

		private static ulong Judge_WriteIndex_(string indexName) =>
			Judge_WriteIndex_ConstBegin + (ulong)indexName.Length;

		private static ulong Judge_WriteIndex_(byte[] indexName) =>
			Judge_WriteIndex_ConstBegin + (ulong)indexName.Length;

		private static ulong Judge_WriteIndex_(Stream indexName) =>
			Judge_WriteIndex_ConstBegin + (ulong)indexName.Length;

		private static ulong Judge_WriteIndex_ConstBegin =>
			1uL + 8uL;

		private static void WriteValue(object value, BinaryWriter sw, GeneratedStream gs) {
			gs.AddValue(value);

			if (value is string) {
				WriteNumber((ulong)((string)value).Length, sw);
				sw.Write(Encoding.UTF8.GetBytes((string)value));
			} else if (value is byte[]) {
				WriteNumber((ulong)((byte[])value).Length, sw);
				sw.Write((byte[])value);
			} else if (value is Stream) {
				WriteNumber((ulong)((Stream)value).Length, sw);

				sw.BaseStream.Flush();

				var s = (Stream)value;
				s.Seek(0, SeekOrigin.Begin);
				sw.BaseStream.Seek(0, SeekOrigin.End);
				
				s.CopyTo(sw.BaseStream);
			}
		}

		private static ulong Judge_WriteValue(object value) =>
			(value is string ?
				Judge_WriteNumber((ulong)((string)value).Length)
				: value is byte[] ?
					Judge_WriteNumber((ulong)((byte[])value).Length)
					: Judge_WriteNumber((ulong)((Stream)value).Length))
			+ (value is string ?
				(ulong)((string)value).Length
				: value is byte[] ?
					(ulong)((byte[])value).Length
					: (ulong)((Stream)value).Length);


		private static void WriteIndexSeperator(ulong nextSeperator, BinaryWriter sw) {
			sw.Write((byte)Consts.IndexSeperator);
			sw.Write((ulong)nextSeperator);
		}

		public static ulong Judge_WriteIndexSeperator => 1uL + 8uL;

		private static void WriteNumber(ulong num, BinaryWriter sw) {
			if (num <= Byte.MaxValue) {
				sw.Write(Consts.IsByteValue);
				sw.Write((byte)num);
			} else if (num <= UInt16.MaxValue) {
				sw.Write(Consts.IsUShortValue);
				sw.Write((ushort)num);
			} else if (num <= UInt32.MaxValue) {
				sw.Write(Consts.IsUIntValue);
				sw.Write((uint)num);
			} else {
				sw.Write(Consts.IsULongValue);
				sw.Write(num);
			}
		}

		private static ulong Judge_WriteNumber(ulong num) =>
			num <= Byte.MaxValue ?
				1uL + 1uL
				: num <= UInt16.MaxValue ?
					1uL + 2uL
					: num <= UInt32.MaxValue ?
						1uL + 4uL
						: 1uL + 8uL;
	}

	public class GeneratedStream {
		public GeneratedStream() { }

		public void AddIndex(string index, ulong dataPos) {
			var newInd = new string[this.Indexes.Length + 1];
			this.Indexes.CopyTo(newInd, 0);

			newInd[newInd.Length - 1] = index ?? throw new ArgumentNullException("index");

			this.Indexes = newInd;

			var newDP = new ulong[this.DataPos.Length + 1];
			this.DataPos.CopyTo(newDP, 0);

			newDP[this.DataPos.Length] = dataPos;

			this.DataPos = newDP;
		}

		public void AddValue(object value) {
			if (!(
				value is string ||
				value is byte[] ||
				value is Stream))
				throw new ArgumentException("Must be a string, byte[], or Stream", "value");

			var newVal = new object[this.Values.Length + 1];
			this.Values.CopyTo(newVal, 0);

			newVal[newVal.Length - 1] = value ?? throw new ArgumentNullException("value");

			this.Values = newVal;
		}

		public StreamData<object> StreamDat { get; set; }
		public Stream Stream { get; set; }
		public string[] Indexes { get; set; } = new string[0];
		public ulong[] DataPos { get; set; } = new ulong[0];
		public object[] Values { get; set; } = new object[0];
		public ulong Overhead { get; set; }

		public void CompareAgainst(Stream other) {
			Assert.True(this.Stream.Length == other.Length, $"The streams lengths aren't equal! ({this.Stream.Length}) v.s. ({other.Length})");

			this.Stream.Seek(0, SeekOrigin.Begin);
			other.Seek(0, SeekOrigin.Begin);

			var right = new List<char>();

			for(var i = 0; i < this.Stream.Length; i++) {
				var byteA = this.Stream.ReadByte();
				var byteB = other.ReadByte();

				Assert.True(byteA == byteB, $"The streams did not match at '{i}'. Stream[{i}] = ({byteA}), other[{i}] = ({byteB}) . . . {GetCharList(right)}");

				right.Add(Convert.ToChar(byteA));
			}
		}

		private string GetCharList(List<char> chrs) {
			var strb = new StringBuilder();

			strb.Append("[");

			foreach (var i in chrs)
				strb.Append(i);

			strb.Append("]");

			return strb.ToString();
		}
	}
}