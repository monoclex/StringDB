using StringDB.IO;

using System.Collections.Generic;

namespace StringDB.Database
{
	public sealed class IODatabase : BaseDatabase<byte[], byte[]>
	{
		private sealed class LazyLoadValue : ILazyLoading<byte[]>
		{
			private readonly IDatabaseIODevice _dbIODevice;
			private readonly long _position;

			public LazyLoadValue(IDatabaseIODevice dbIODevice, long position)
			{
				_dbIODevice = dbIODevice;
				_position = position;
			}

			public byte[] Load() => _dbIODevice.ReadValue(_position);
		}

		private readonly IDatabaseIODevice _dbIODevice;

		public IODatabase(IDatabaseIODevice dbIODevice) => _dbIODevice = dbIODevice;

		public override void InsertRange(KeyValuePair<byte[], byte[]>[] items) => _dbIODevice.Insert(items);

		protected override IEnumerable<KeyValuePair<byte[], ILazyLoading<byte[]>>> Evaluate()
		{
			_dbIODevice.Reset();

			DatabaseItem dbItem;

			while (!(dbItem = _dbIODevice.ReadNext()).IsLast)
			{
				yield return new KeyValuePair<byte[], ILazyLoading<byte[]>>
				(
					key: dbItem.Key,
					value: new LazyLoadValue(_dbIODevice, dbItem.ValuePosition)
				);
			}
		}

		public override void Dispose() => _dbIODevice.Dispose();
	}
}