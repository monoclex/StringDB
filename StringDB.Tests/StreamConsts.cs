using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StringDB.Tests {
	public static class StreamConsts {
		public static Stream BlankStream() =>
			new MemoryStream();

		public static GeneratedStream OnePerOne(string index, object value) {
			//_one_ value per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(index, (ulong)s.Position + Judge_WriteIndexSeperator + Judge_WriteIndex(index), sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(value, sw, gs);
			sw.Flush();
			return gs;

		}

		public static GeneratedStream TwoPerOne(string index1, object value1, string index2, object value2) {
			//_two_ values per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(index1, (ulong)s.Position + Judge_WriteIndex(index1) + Judge_WriteIndex(index2) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndex(index2, (ulong)s.Position + Judge_WriteIndex(index2) + Judge_WriteIndexSeperator + Judge_WriteValue(value1), sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(value1, sw, gs);
			WriteValue(value2, sw, gs);
			sw.Flush();
			return gs;

		}

		public static GeneratedStream OnePerTwo(string index1, object value1, string index2, object value2) {
			//_two_ values per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(index1, (ulong)s.Position + Judge_WriteIndex(index1) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndexSeperator((ulong)s.Position + Judge_WriteIndexSeperator + Judge_WriteValue(value1), sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(value1, sw, gs);
			WriteIndex(index2, (ulong)s.Position + Judge_WriteIndex(index2) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndexSeperator(0, sw);
			WriteValue(value2, sw, gs);
			sw.Flush();
			return gs;

		}

		public static GeneratedStream TwoPerTwo(string index1, object value1, string index2, object value2, string index3, object value3, string index4, string value4) {
			//_two_ values per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex(index1, (ulong)s.Position + Judge_WriteIndex(index1) + Judge_WriteIndex(index2) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndex(index2, (ulong)s.Position + Judge_WriteIndex(index2) + Judge_WriteIndexSeperator + Judge_WriteValue(value1), sw, gs);
			WriteIndexSeperator((ulong)s.Position + Judge_WriteIndexSeperator + Judge_WriteValue(value1) + Judge_WriteValue(value2), sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(value1, sw, gs);
			WriteValue(value2, sw, gs);
			WriteIndex(index3, (ulong)s.Position + Judge_WriteIndex(index3) + Judge_WriteIndex(index4) + Judge_WriteIndexSeperator, sw, gs);
			WriteIndex(index4, (ulong)s.Position + Judge_WriteIndex(index4) + Judge_WriteIndexSeperator + Judge_WriteValue(value3), sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue(value3, sw, gs);
			WriteValue(value4, sw, gs);
			sw.Flush();
			return gs;

		}

		private static void WriteIndex(string indexName, ulong dataPos, BinaryWriter sw, GeneratedStream gs) {
			sw.Write((byte)(indexName.Length));
			sw.Write((ulong)dataPos);
			sw.Write(Encoding.UTF8.GetBytes(indexName));

			gs.AddIndex(indexName);
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

		public void AddIndex(string index) {
			var newInd = new string[this.Indexes.Length + 1];
			this.Indexes.CopyTo(newInd, 0);

			newInd[newInd.Length - 1] = index ?? throw new ArgumentNullException("index");

			this.Indexes = newInd;
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

		public Stream Stream { get; set; }
		public string[] Indexes { get; set; } = new string[0];
		public object[] Values { get; set; } = new object[0];

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