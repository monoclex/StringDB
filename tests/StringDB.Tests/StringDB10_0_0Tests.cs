using FluentAssertions;

using Moq;

using StringDB.IO;
using StringDB.IO.Compatibility;

using System;
using System.IO;
using System.Linq;
using System.Text;

using Xunit;

namespace StringDB.Tests
{
	// literally a final boss lmao
	// will come back to this later :eyes:

	// lmao messy code
	// TODO: clean
	public static class StringDB10_0_0Tests
	{
		public static (MemoryStream ms, StringDB10_0_0LowlevelDatabaseIODevice io) Generate()
		{
			var ms = new MemoryStream();
			var io = new StringDB10_0_0LowlevelDatabaseIODevice(ms, true);

			return (ms, io);
		}

		public class Exceptions
		{
			[Fact]
			public void ThrowsOnVeryLongVariableIntegerRead()
			{
				var (ms, io) = Generate();
				ms.Write(new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

				Action throws = () => io.ReadValue(8);

				throws.Should()
					.ThrowExactly<FormatException>();
			}

			[Fact]
			public void NotIndexChainOnReadJump()
			{
				var (ms, io) = Generate();

				ms.Write(new byte[1] { 0x00 });
				io.Reset();

				Action throws = () => io.ReadJump();

				throws.Should()
					.ThrowExactly<EndOfStreamException>();
			}

			[Fact]
			public void TooLargeJumpRead()
			{
				var (ms, io) = Generate();

				Action throws = () => io.WriteJump(0xDEAD_BEEF_5);

				throws.Should()
					.ThrowExactly<ArgumentException>();
			}

			[Fact]
			public void AtEOF()
			{
				var (ms, io) = Generate();

				Action throws = () => io.ReadIndex(0);

				throws.Should()
					.ThrowExactly<NotSupportedException>();
			}

			[Fact]
			public void ReadingVeryLargeVariableInt()
			{
				var (ms, io) = Generate();

				Action throws = () => io.ReadValue(8);

				io.Reset();
				ms.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0b0_1111111, 0x00 });

				throws.Should()
					.ThrowExactly<NotSupportedException>();
			}
		}

		public static class Write
		{
			public class Index
			{
				[Fact]
				public void LongBoi()
				{
					var (ms, io) = Generate();

					Action throws = () => io.WriteIndex(new byte[1337], 0);

					throws.Should()
						.ThrowExactly<ArgumentException>();
				}

				[Fact]
				public void ShortBoi()
				{
					var (ms, io) = Generate();

					Action throws = () => io.WriteIndex(new byte[0], 0);

					throws.Should()
						.ThrowExactly<ArgumentException>();
				}

				[Fact]
				public void Format()
				{
					var (ms, io) = Generate();

					var val = new byte[] { 0xAA, 0xBB };
					io.WriteIndex(val, 0xBEEF);

					ms.Position = 8;
					var data = new byte[io.CalculateIndexOffset(val)];
					ms.Read(data, 0, data.Length);

					data
						.Should()
						.BeEquivalentTo(new byte[]
						{
							// index length, as a single byte
							0x02,

							// data position

							// E6 and not EF because E6 - 10(dec), and
							// the jump pos is relative to our location;
							// which is 9 (the index length + 8 beginning bytes)
							0xE6, 0xBE, 0x00, 0x00,

							//the actual index
							0xAA, 0xBB
						});
				}
			}

			public class Value
			{
				[Fact]
				public void ShortValue()
				{
					var (ms, io) = Generate();

					var val = new byte[] { 0xDE, 0xAD, 0x00, 0xBE, 0xEF };
					io.WriteValue(val);

					ms.Position = 8;
					var data = new byte[io.CalculateValueOffset(val)];
					ms.Read(data, 0, data.Length);

					data
						.Should()
						.BeEquivalentTo(new byte[]
						{
							// 5 in binary,
							// leading bit off
							0b0_0000101,

							0xDE, 0xAD, 0x00, 0xBE, 0xEF
						});
				}

				[Fact]
				public void LongValue()
				{
					var (ms, io) = Generate();

					var val = new byte[12345];
					io.WriteValue(val);

					ms.Position = 8;
					var data = new byte[io.CalculateValueOffset(val)];
					ms.Read(data, 0, data.Length);

					data
						.Should()
						.BeEquivalentTo(new byte[]
						{
							// 12345 in binary is "0011 0000 0011 1001"
							// this is the variable int format
							0b1_011_1001, 0b0_110_0000
						}.Concat(val));
				}
			}

			public class Jump
			{
				[Fact]
				public void JumpShortened()
				{
					var (ms, io) = Generate();

					io.WriteJump(1337);

					const int pos = 9;

					ms.Position = pos - 1; // to read the index separator
					var bytes = new byte[4];
					ms.ReadByte()
						.Should()
						.Be(0xFF, "Index separator");
					ms.Read(bytes, 0, 4);

					BitConverter.ToInt32(bytes)
						.Should()
						.Be(1337 - pos);
				}

				[Fact]
				public void WritesJumpPosition()
				{
					var (ms, io) = Generate();

					io.JumpPos = 0xDEADBEEF5;

					io.Dispose();

					ms.Position = 0;
					var jumpPos = new byte[8];
					ms.Read(jumpPos, 0, jumpPos.Length);

					jumpPos.Should().BeEquivalentTo(BitConverter.GetBytes(0xDEADBEEF5));
				}
			}
		}

		public static class Read
		{
			public class Index
			{
				[Fact]
				public void ReadsIndexOk()
				{
					var (ms, io) = Generate();

					io.WriteIndex(Encoding.UTF8.GetBytes("key"), 1337);

					io.Reset();

					io.Peek(out var peekResult)
						.Should().Be(NextItemPeek.Index);

					var index = io.ReadIndex(peekResult);

					index.Index
						.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("key"));

					index.DataPosition
						.Should().Be(1337);
				}
			}

			public class Value
			{
				[Fact]
				public void ReadsValueOk()
				{
					var (ms, io) = Generate();

					const int len = 12345;

					io.WriteValue(new byte[len]);

					io.ReadValue(8)
						.Should()
						.BeEquivalentTo(Enumerable.Repeat<byte>(0x00, len));
				}
			}

			public static class Peek
			{
				public class EOF
				{
					[Fact]
					public void PeeksEOFFine()
					{
						var (ms, io) = Generate();

						io.Peek(out var peekResult1)
							.Should().Be(NextItemPeek.EOF);

						peekResult1.Should().Be(0x00);

						ms.Write(new byte[88]);
						io.Reset();

						io.Peek(out var peekResult2)
							.Should().Be(NextItemPeek.EOF);

						peekResult2.Should().Be(0x00);
					}
				}

				public class Jump
				{
					[Fact]
					public void PeeksJumpFine()
					{
						var (ms, io) = Generate();

						io.WriteJump(1337);
						io.Reset();

						io.Peek(out var peekResult).Should().Be(NextItemPeek.Jump);

						io.ReadJump().Should().Be(1337);

						peekResult.Should().Be(0xFF);
					}
				}

				public class PeekElse
				{
					[Fact]
					public void PeeksElse()
					{
						var (ms, io) = Generate();

						ms.WriteByte(0x12);
						io.Reset();

						io.Peek(out var peekResult)
							.Should().Be(NextItemPeek.Index);

						peekResult.Should().Be(0x12);
					}
				}
			}
		}

		public class SizeCorrelations
		{
			[Fact]
			public void Index()
			{
				var (ms, io) = Generate();
				var key = new byte[8];

				io.WriteIndex(key, 1234);

				ms.Length
					.Should()
					.Be(sizeof(long) + io.CalculateIndexOffset(key));
			}

			[Fact]
			public void Value()
			{
				var (ms, io) = Generate();
				var value = Enumerable.Repeat<byte>(0xFF, 16).ToArray();

				io.WriteValue(value);

				io.Flush();

				ms.Length
					.Should()
					.Be(sizeof(long) + io.CalculateValueOffset(value));
			}

			[Fact]
			public void Jump()
			{
				var (ms, io) = Generate();

				io.WriteJump(81309);

				ms.Length
					.Should()
					.Be(sizeof(long) + io.JumpOffsetSize);
			}
		}

		public class DeadSimple
		{
			[Fact]
			public void JumpOffsetSize()
				=> Generate()
					.io.JumpOffsetSize
					.Should()
					.Be(sizeof(byte) + sizeof(int));

			[Fact]
			public void ReadsLongPrefix()
			{
				using (var ms = new MemoryStream())
				using (var io = new StringDB10_0_0LowlevelDatabaseIODevice(ms))
				{
					io.JumpPos
						.Should()
						.Be(0);

					ms.Length
						.Should()
						.Be(8);
				}

				using (var ms = new MemoryStream())
				{
					ms.Write(new byte[8] { 123, 0, 0, 0, 0, 0, 0, 0 });

					using (var io = new StringDB10_0_0LowlevelDatabaseIODevice(ms))
					{
						io.JumpPos
							.Should()
							.Be(123);

						ms.Length
							.Should()
							.Be(8);
					}
				}
			}

			[Fact]
			public void GetPosition()
			{
				var (ms, io) = Generate();
				ms.Write(new byte[100]);

				io.Seek(20);
				ms.Position.Should().Be(20);

				io.GetPosition()
					.Should()
					.Be(20);

				io.Seek(40);
				ms.Position.Should().Be(40);

				io.GetPosition()
					.Should()
					.Be(40);
			}

			[Fact]
			public void Reset()
			{
				var (ms, io) = Generate();
				ms.Write(new byte[100]);

				io.Reset();

				ms.Position
					.Should()
					.Be(sizeof(long));
			}

			[Fact]
			public void Seek()
			{
				var (ms, io) = Generate();
				ms.Write(new byte[100]);

				io.Seek(3);
				ms.Position
					.Should()
					.Be(3);

				io.Seek(38);
				ms.Position
					.Should()
					.Be(38);

				io.Seek(62);
				ms.Position
					.Should()
					.Be(62);
			}

			[Fact]
			public void SeekEnd()
			{
				var (ms, io) = Generate();

				io.SeekEnd();
				ms.Position
					.Should()
					.Be(ms.Length);

				ms.Write(new byte[1]);

				io.SeekEnd();
				ms.Position
					.Should()
					.Be(ms.Length);

				ms.Write(new byte[1]);

				io.SeekEnd();
				ms.Position
					.Should()
					.Be(ms.Length);

				ms.Write(new byte[100]);

				io.SeekEnd();
				ms.Position
					.Should()
					.Be(ms.Length);
			}

			[Fact]
			public void Flush()
			{
				var mock = new Mock<Stream>();

				var flushed = 0;
				mock.Setup(stream => stream.CanRead).Returns(true);
				mock.Setup(stream => stream.CanWrite).Returns(true);
				mock.Setup(stream => stream.Flush()).Callback(() => flushed++);

				var io = new StringDB10_0_0LowlevelDatabaseIODevice(mock.Object);

				io.Flush();

				// binary writer flushes and so does the stream
				flushed.Should().Be(2);
			}
		}
	}
}