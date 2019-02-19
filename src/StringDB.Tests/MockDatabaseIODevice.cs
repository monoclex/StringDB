using StringDB.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringDB.Tests
{
	public class MockDatabaseIODevice : IDatabaseIODevice, IMockDatabase<byte[], byte[]>
	{
		public List<KeyValuePair<byte[], LazyItem<byte[]>>> Data { get; } = new List<KeyValuePair<string, string>>()
		{
			{ new KeyValuePair<string, string>("a", "b") },
			{ new KeyValuePair<string, string>("c", "d") },
			{ new KeyValuePair<string, string>("e", "e") },
			{ new KeyValuePair<string, string>("e", "g") },
			{ new KeyValuePair<string, string>("g", "g") },
		}
		.Select(x =>
		{
			return new KeyValuePair<byte[], LazyItem<byte[]>>
			(
				key: Encoding.UTF8.GetBytes(x.Key),
				value: new LazyItem<byte[]>(Encoding.UTF8.GetBytes(x.Value))
			);
		})
		.ToList();

		public int ItemOn = -1;

		public void Reset() => ItemOn = 0;

		public KeyValuePair<byte[], byte[]>[] Inserted;

		public void Insert(KeyValuePair<byte[], byte[]>[] items) => Inserted = items;

		public DatabaseItem ReadNext()
		{
			var current = ItemOn++;

			return new DatabaseItem
			{
				IsLast = ItemOn == Data.Count,
				Key = Data[current].Key,
				ValuePosition = current
			};
		}

		public long RequestedRead = 0;

		public byte[] ReadValue(long position)
		{
			RequestedRead = position;

			return Data[(int)position].Value.Load();
		}

		public bool Disposed;

		public void Dispose() => Disposed = true;
	}
}