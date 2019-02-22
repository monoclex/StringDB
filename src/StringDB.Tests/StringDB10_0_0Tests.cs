using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Moq;
using StringDB.IO.Compatability;
using Xunit;

namespace StringDB.Tests
{
	public class StringDB10_0_0Tests
	{
		public static (MemoryStream ms, StringDB10_0_0LowlevelDatabaseIODevice io) Generate()
		{
			var ms = new MemoryStream();
			var io = new StringDB10_0_0LowlevelDatabaseIODevice(ms);

			return (ms, io);
		}

		public class Write
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
				var value = new byte[16];

				io.WriteValue(value);

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

				ms.Position = 20;

				io.GetPosition()
					.Should()
					.Be(20);

				ms.Position = 40;
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
