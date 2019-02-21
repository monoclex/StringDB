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
	public class StoneVaultIODeviceTests
	{
		[Fact]
		public void InsertWorks()
		{
			var ms = new MemoryStream();
			var sviod = new StoneVaultIODevice(ms, false);

			sviod.Insert(new KeyValuePair<byte[], byte[]>[]
			{
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key"), Encoding.UTF8.GetBytes("value"))
			});

			ms.ToArray()
				.Should()
				.BeEquivalentTo(new byte[]
				{
					0x00, // DATA_GOOD
					3, 0, 0, 0, 0, 0, 0, 0, // 3
				}
				.Concat(Encoding.UTF8.GetBytes("key"))
				.Concat(new byte[]
				{
					0x00, // DATA_GOOD
					5, 0, 0, 0, 0, 0, 0, 0, // 5
				})
				.Concat(Encoding.UTF8.GetBytes("value"))
				.Concat(new byte[]
				{
					0xFF // DATA_BAD
				}));

			sviod.Insert(new KeyValuePair<byte[], byte[]>[]
			{
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("a"))
			});

			ms.ToArray()
				.Should()
				.BeEquivalentTo(new byte[]
				{
					0x00, // DATA_GOOD
					3, 0, 0, 0, 0, 0, 0, 0, // 3
				}
				.Concat(Encoding.UTF8.GetBytes("key"))
				.Concat(new byte[]
				{
					0x00, // DATA_GOOD
					5, 0, 0, 0, 0, 0, 0, 0, // 5
				})
				.Concat(Encoding.UTF8.GetBytes("value"))
				.Concat(new byte[]
				{
					0x00, // DATA_GOOD
					1, 0, 0, 0, 0, 0, 0, 0, // 1
				})
				.Concat(Encoding.UTF8.GetBytes("a"))
				.Concat(new byte[]
				{
					0x00, // DATA_GOOD
					1, 0, 0, 0, 0, 0, 0, 0, // 1
				})
				.Concat(Encoding.UTF8.GetBytes("a"))
				.Concat(new byte[]
				{
					0xFF // DATA_BAD
				}));
		}

		[Fact]
		public void CanRead()
		{
			var ms = new MemoryStream();
			var sviod = new StoneVaultIODevice(ms, false);

			sviod.Insert(new KeyValuePair<byte[], byte[]>[]
			{
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("test"), Encoding.UTF8.GetBytes("value")),
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("test2"), Encoding.UTF8.GetBytes("value2")),
			});

			sviod.Reset();

			var key1 = sviod.ReadNext();
			var key2 = sviod.ReadNext();
			var key3 = sviod.ReadNext();

			// works fine
			key1.EndOfItems.Should().BeFalse();
			key2.EndOfItems.Should().BeFalse();
			key3.EndOfItems.Should().BeTrue();

			key1.Key.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("test"));
			key2.Key.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("test2"));

			sviod.ReadValue(key1.DataPosition)
				.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("value"));

			sviod.ReadValue(key2.DataPosition)
				.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("value2"));

			sviod.Reset();

			// ensure that the stream seeks back to where we were reading when we read a value
			var pos = sviod.ReadNext().DataPosition;
			var value = sviod.ReadValue(pos);
			var key2_clone = sviod.ReadNext();

			key2_clone
				.Should().BeEquivalentTo(key2);
		}
	}
}
