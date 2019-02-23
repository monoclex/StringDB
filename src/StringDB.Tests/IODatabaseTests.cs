using FluentAssertions;

using StringDB.Databases;

using System.Collections.Generic;
using System.Text;

using Xunit;

namespace StringDB.Tests
{
	public class IODatabaseTests
	{
		[Fact]
		public void Enumerates()
		{
			var mdbiod = new MockDatabaseIODevice();
			var iodb = new IODatabase(mdbiod);

			bool first = true;

			int i = 0;
			foreach (var item in iodb)
			{
				if (first)
				{
					// 1 because it's enumerated by 1 already

					mdbiod.ItemOn
						.Should()
						.Be(1, "Reset() called by the IODatabase");

					first = false;
				}

				item.Key
					.Should()
					.BeEquivalentTo(mdbiod.Data[i].Key);

				var lazyItem = mdbiod.Data[i].Value;

				lazyItem.Loaded
					.Should()
					.BeFalse();

				item.Value.Load()
					.Should()
					.BeEquivalentTo(lazyItem.Value);

				lazyItem.Loaded
					.Should()
					.BeTrue();

				i++;
			}
		}

		[Fact]
		public void Inserts()
		{
			var mdbiod = new MockDatabaseIODevice();
			var iodb = new IODatabase(mdbiod);

			var inserting = new KeyValuePair<byte[], byte[]>[]
			{
				new KeyValuePair<byte[], byte[]>
				(
					key: Encoding.UTF8.GetBytes("test"),
					value: Encoding.UTF8.GetBytes("value")
				)
			};

			iodb.InsertRange(inserting);

			mdbiod.Inserted
				.Should()
				.BeEquivalentTo(inserting);
		}
	}
}