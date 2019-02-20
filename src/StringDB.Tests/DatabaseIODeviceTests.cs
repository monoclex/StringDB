using FluentAssertions;

using StringDB.IO;

using System;
using System.Collections.Generic;
using System.IO;
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

		public void WriteBytes(BinaryWriter bw, string str)
			=> bw.Write(Encoding.UTF8.GetBytes(str));
	}
}