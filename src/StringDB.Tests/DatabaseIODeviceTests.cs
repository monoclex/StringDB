using FluentAssertions;

using StringDB.IO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xunit;

namespace StringDB.Tests
{
	public class DatabaseIODeviceTests
	{
		public class InsertsProperlyMock : ILowlevelDatabaseIODevice
		{
			public InsertsProperlyMock()
			{
				Stream = new MemoryStream();
				BW = new BinaryWriter(Stream);
				BR = new BinaryReader(Stream);
			}

			public MemoryStream Stream { get; }
			public BinaryWriter BW { get; }
			public BinaryReader BR { get; }

			public void Dispose() => throw new NotImplementedException();

			public void Flush() => throw new NotImplementedException();

			public NextItemPeek Peek() => throw new NotImplementedException();

			public LowLevelDatabaseItem ReadIndex() => throw new NotImplementedException();

			public long ReadJump() => throw new NotImplementedException();

			public byte[] ReadValue(long dataPosition) => throw new NotImplementedException();

			public void Reset() => throw new NotImplementedException();

			public long JumpPos { get; set; }

			public int CalculateValueOffset(byte[] value) => value.Length;

			public void WriteIndex(byte[] key, long dataPosition)
			{
				BW.Write(key);
				BW.Write(dataPosition);
			}

			public void WriteValue(byte[] value) => BW.Write(value);

			public int JumpOffsetSize => "JMP_xxxxxxxx".Length;

			public int CalculateIndexOffset(byte[] key) => key.Length + sizeof(long);

			public void WriteJump(long jumpTo)
			{
				BW.Write(Encoding.UTF8.GetBytes("JMP_"));
				BW.Write(jumpTo);
			}

			public void Seek(long position) => Stream.Seek(position, SeekOrigin.Begin);

			public long GetPosition() => Stream.Position;

			public void SeekEnd() => Stream.Seek(0, SeekOrigin.End);
		}

		[Fact]
		public void InsertsProperly()
		{
			var ipm = new InsertsProperlyMock();

			var diod = new DatabaseIODevice(ipm);

			diod.Insert(new KeyValuePair<byte[], byte[]>[3]
			{
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key"), Encoding.UTF8.GetBytes("value")),
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key2"), Encoding.UTF8.GetBytes("value2")),
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key33"), Encoding.UTF8.GetBytes("value33"))
			});

			diod.Insert(new KeyValuePair<byte[], byte[]>[3]
			{
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key"), Encoding.UTF8.GetBytes("value")),
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key2"), Encoding.UTF8.GetBytes("value2")),
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key33"), Encoding.UTF8.GetBytes("value33"))
			});

			File.WriteAllBytes("examine.txt", ipm.Stream.ToArray());

			var ms = new MemoryStream();
			var bw = new BinaryWriter(ms);

			WriteBytes(bw, "key");
			bw.Write(0x30L);
			WriteBytes(bw, "key2");
			bw.Write(0x35L);
			WriteBytes(bw, "key33");
			bw.Write(0x3BL);
			WriteBytes(bw, "JMP_");
			bw.Write(0x42L);
			WriteBytes(bw, "value");
			WriteBytes(bw, "value2");
			WriteBytes(bw, "value33");
			WriteBytes(bw, "key");
			bw.Write(0x72L);
			WriteBytes(bw, "key2");
			bw.Write(0x77L);
			WriteBytes(bw, "key33");
			bw.Write(0x7DL);
			WriteBytes(bw, "JMP_");
			bw.Write(0x0L);
			WriteBytes(bw, "value");
			WriteBytes(bw, "value2");
			WriteBytes(bw, "value33");
			bw.Flush();

			ipm.Stream.ToArray()
				.Should()
				.BeEquivalentTo(ms.ToArray());
		}

		private static void WriteBytes(BinaryWriter bw, string str)
			=> bw.Write(Encoding.UTF8.GetBytes(str));

		public class EnumeratesMock : ILowlevelDatabaseIODevice
		{
			public long JumpPos { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
			public int JumpOffsetSize => throw new NotImplementedException();

			public int CalculateIndexOffset(byte[] key) => throw new NotImplementedException();

			public int CalculateValueOffset(byte[] value) => throw new NotImplementedException();

			public void Dispose() => throw new NotImplementedException();

			public void Flush() => throw new NotImplementedException();

			public long GetPosition() => 0;

			public byte[] ReadValue(long dataPosition)
			{
				Seek(dataPosition);
				return Data.First(x => x.Position == dataPosition).Value;
			}

			public void Reset() => throw new NotImplementedException();

			public void SeekEnd() => throw new NotImplementedException();

			public void WriteIndex(byte[] key, long dataPosition) => throw new NotImplementedException();

			public void WriteJump(long jumpTo) => throw new NotImplementedException();

			public void WriteValue(byte[] value) => throw new NotImplementedException();

			public class SomeData
			{
				public int Position { get; set; }

				public bool SoughtTo { get; set; }

				public byte[] Key { get; set; }
				public byte[] Value { get; set; }
			}

			public SomeData[] Data = new SomeData[]
			{
				new SomeData
				{
					Position = 123,
					Key = Encoding.UTF8.GetBytes("key"),
					Value = Encoding.UTF8.GetBytes("value")
				},
				new SomeData
				{
					Position = 456,
					Key = Encoding.UTF8.GetBytes("key2"),
					Value = Encoding.UTF8.GetBytes("value2")
				},
				new SomeData
				{
					Position = 789,
					Key = Encoding.UTF8.GetBytes("key33"),
					Value = Encoding.UTF8.GetBytes("value33")
				},
			};

			public void Seek(long position)
			{
				// this is because of the GetPosition
				if (position == 0) return;

				var d = Data.First(x => x.Position == position);

				d.SoughtTo = true;
			}

			public NextItemPeek PeekToReturn { get; set; } = NextItemPeek.Index;

			public NextItemPeek Peek()
			{
				if (ArrayIndex == Data.Length) return NextItemPeek.EOF;

				if (PeekToReturn == NextItemPeek.Jump)
				{
					// we will jmp then back
					PeekToReturn = NextItemPeek.Index;

					return NextItemPeek.Jump;
				}

				return PeekToReturn;
			}

			public int ArrayIndex { get; set; } = 0;

			public LowLevelDatabaseItem ReadIndex()
			{
				var data = Data[ArrayIndex];

				var item = new LowLevelDatabaseItem
				{
					DataPosition = data.Position,
					Index = data.Key
				};

				ArrayIndex++;

				return item;
			}

			public long SendJump { get; set; }

			public long ReadJump() => SendJump;
		}

		[Fact]
		public void EnumeratesProperly()
		{
			var em = new EnumeratesMock();

			var diod = new DatabaseIODevice(em);

			var a = diod.ReadNext();

			a.Should()
				.BeEquivalentTo(new DatabaseItem
				{
					DataPosition = 123,
					Key = Encoding.UTF8.GetBytes("key"),
					EndOfItems = false
				});

			var b = diod.ReadNext();

			b.Should()
				.BeEquivalentTo(new DatabaseItem
				{
					DataPosition = 456,
					Key = Encoding.UTF8.GetBytes("key2"),
					EndOfItems = false
				});

			var c = diod.ReadNext();

			c.Should()
				.BeEquivalentTo(new DatabaseItem
				{
					DataPosition = 789,
					Key = Encoding.UTF8.GetBytes("key33"),
					EndOfItems = false
				});

			var d = diod.ReadNext();

			d.EndOfItems
				.Should()
				.BeTrue();
		}

		[Fact]
		public void ExecutesJumpProperly()
		{
			var em = new EnumeratesMock();

			var diod = new DatabaseIODevice(em);

			var a = diod.ReadNext();

			a.Should()
				.BeEquivalentTo(new DatabaseItem
				{
					DataPosition = 123,
					Key = Encoding.UTF8.GetBytes("key"),
					EndOfItems = false
				});

			em.SendJump = 789;
			em.PeekToReturn = NextItemPeek.Jump;
			em.ArrayIndex = 2;

			var b = diod.ReadNext();

			b.Should()
				.BeEquivalentTo(new DatabaseItem
				{
					DataPosition = 789,
					Key = Encoding.UTF8.GetBytes("key33"),
					EndOfItems = false
				});

			var d = diod.ReadNext();

			d.EndOfItems
				.Should()
				.BeTrue();
		}

		[Fact]
		public void ReadsValueFine()
		{
			var em = new EnumeratesMock();

			var diod = new DatabaseIODevice(em);

			diod.ReadValue(123)
				.Should()
				.BeEquivalentTo(Encoding.UTF8.GetBytes("value"));

			em.Data[0].SoughtTo
				.Should()
				.BeTrue();
		}
	}
}