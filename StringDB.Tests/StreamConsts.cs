using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace StringDB.Tests {
	public class test {
		[Fact]
		public void Test() {
			var ms = new MemoryStream();
			var w = new StringDB.Writer.StreamWriter(ms, DatabaseVersion.Latest, true);

			//w.InsertRange(new List<KeyValuePair<string, string>>(){
			//	new KeyValuePair<string, string>("Test1", "TestValue1"),
			//	new KeyValuePair<string, string>("Test2", "TestValue2")
			//});

			//var gs = StreamConsts.TwoPerOne();

			w.Insert("Test1", "TestValue1");

			var gs = StreamConsts.OnePerOne();

			ms.Flush();
			gs.CompareAgainst(ms);
		}
	}

	public static class StreamConsts {
		public static Stream BlankStream() =>
			new MemoryStream();

		public static GeneratedStream OnePerOne() {
			//_one_ value per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex("Test1", (ulong)s.Position + Judge_WriteIndex("Test1") + Judge_WriteIndexSeperator, sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue("TestValue1", sw, gs);
			sw.Flush();
			return gs;

		}

		public static GeneratedStream TwoPerOne() {
			//_two_ values per _one_ index chain
			var s = new MemoryStream();
			var gs = new GeneratedStream() {
				Stream = s
			};

			var sw = new BinaryWriter(s);

			s.Seek(0, SeekOrigin.Begin);
			WriteIndex("Test1", (ulong)s.Position + Judge_WriteIndex("Test1") + Judge_WriteIndex("Test2") + Judge_WriteIndexSeperator, sw, gs);
			WriteIndex("Test2", (ulong)s.Position + Judge_WriteIndex("Test2") + Judge_WriteIndexSeperator + Judge_WriteValue("TestValue1"), sw, gs);
			WriteIndexSeperator(0, sw); // (ulong)s.Position + Judge_WriteIndexSeperator, sw);
			WriteValue("TestValue1", sw, gs);
			WriteValue("TestValue2", sw, gs);
			sw.Flush();
			return gs;

		}

		private static void WriteIndex(string indexName, ulong dataPos, BinaryWriter sw, GeneratedStream gs) {
			sw.Write((byte)(indexName.Length));
			sw.Write((ulong)dataPos);
			sw.Write(Encoding.UTF8.GetBytes(indexName));

			gs.AddIndex(indexName);
		}

		private static ulong Judge_WriteIndex(string indexName) =>
			Judge_WriteIndex_ConstBegin + (ulong)indexName.Length;

		private static ulong Judge_WriteIndex(byte[] indexName) =>
			Judge_WriteIndex_ConstBegin + (ulong)indexName.Length;

		private static ulong Judge_WriteIndex(Stream indexName) =>
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
			//Assert.True(this.Stream.Length == other.Length, $"The streams lengths aren't equal! ({this.Stream.Length}) v.s. ({other.Length})");

			this.Stream.Seek(0, SeekOrigin.Begin);
			other.Seek(0, SeekOrigin.Begin);

			for(var i = 0; i < this.Stream.Length; i++) {
				var byteA = this.Stream.ReadByte();
				var byteB = other.ReadByte();

				Assert.True(byteA == byteB, $"The streams did not match at '{i}'. Stream[{i}] = ({byteA}), other[{i}] = ({byteB})");
			}
		}
	}
}