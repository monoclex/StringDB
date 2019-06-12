using FluentAssertions;

using StringDB.IO;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests that a StoneVaultIODevice writes the correct bytes.
	/// </summary>
	public class StoneVaultIODeviceTests
	{
		private readonly MemoryStream _ms;
		private readonly StoneVaultIODevice _sviod;

		public StoneVaultIODeviceTests()
		{
			_ms = new MemoryStream();
			_sviod = new StoneVaultIODevice(_ms);
		}

		/// <summary>
		/// Tests that inserts work.
		/// </summary>
		[Fact]
		public void InsertWorks()
		{
			// insert key/value
			_sviod.Insert(new KeyValuePair<byte[], byte[]>[]
			{
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("key"), Encoding.UTF8.GetBytes("value"))
			});

			// tests file format
			_ms.ToArray()
				.Should()
				.BeEquivalentTo(TestKeyValue().Concat(DataBad()));

			_sviod.Insert(new KeyValuePair<byte[], byte[]>[]
			{
				new KeyValuePair<byte[], byte[]>(Encoding.UTF8.GetBytes("a"), Encoding.UTF8.GetBytes("a"))
			});

			_ms.ToArray()
				.Should()
				.BeEquivalentTo(TestKeyValue().Concat(TestAA()).Concat(DataBad()));

			static IEnumerable<byte> TestKeyValue()
			{
				return new byte[]
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
				   .Concat(Encoding.UTF8.GetBytes("value"));
			}

			static IEnumerable<byte> TestAA()
			{
				return new byte[]
				{
					0x00, // DATA_GOOD
					1, 0, 0, 0, 0, 0, 0, 0, // 1
				}
				.Concat(Encoding.UTF8.GetBytes("a"))
				.Concat(new byte[]
				{
					0x00, // DATA_GOOD
					1, 0, 0, 0, 0, 0, 0, 0, // 1
				})
				.Concat(Encoding.UTF8.GetBytes("a"));
			}

			static IEnumerable<byte> DataBad()
			{
				return new byte[]
				{
					0xFF // DATA_BAD
				};
			}
		}

		/// <summary>
		/// Tests that reading works.
		/// </summary>
		[Fact]
		public void CanRead()
		{
			// insert two entries
			_sviod.Insert(new KeyValuePair<byte[], byte[]>[]
			{
				new KeyValuePair<byte[], byte[]>(Bytes("test"), Bytes("value")),
				new KeyValuePair<byte[], byte[]>(Bytes("test2"), Bytes("value2")),
			});

			// reset the head
			_sviod.Reset();

			// read some keys
			var keys = new[]
			{
				_sviod.ReadNext(),
				_sviod.ReadNext(),
				_sviod.ReadNext(),
			};

			foreach (var key in keys.Take(2))
			{
				key.EndOfItems.Should().BeFalse();
			}

			// TODO: use hat ^
			keys[keys.Length - 1].EndOfItems.Should().BeTrue();

			// now test reading key and value
			keys[0].Key.Should().BeEquivalentTo(Bytes("test"));
			keys[1].Key.Should().BeEquivalentTo(Bytes("test2"));

			_sviod.ReadValue(keys[0].DataPosition)
				.Should().BeEquivalentTo(Bytes("value"));

			_sviod.ReadValue(keys[1].DataPosition)
				.Should().BeEquivalentTo(Bytes("value2"));

			_sviod.Reset();

			// ensure that the stream seeks back to where we were reading when we read a value
			var pos = _sviod.ReadNext().DataPosition;

			// when we read a value, the head should go to the value at 2 now
			_sviod.ReadValue(pos);

			_sviod.ReadNext()
				.Should().BeEquivalentTo(keys[1]);

			static byte[] Bytes(string str) => Encoding.UTF8.GetBytes(str);
		}
	}
}