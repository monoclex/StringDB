using StringDB.IO;

using System.Collections.Generic;

namespace StringDB.Databases
{
	/// <summary>
	/// A database for IO-based operations, using an IDatabaseIODevice.
	/// </summary>
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

		/// <summary>
		/// Create an IODatabase with the IDatabaseIODevice specified.
		/// </summary>
		/// <param name="dbIODevice">The DatabaseIODevice to use under the hood.</param>
		public IODatabase(IDatabaseIODevice dbIODevice) => _dbIODevice = dbIODevice;

		public override void InsertRange(KeyValuePair<byte[], byte[]>[] items) => _dbIODevice.Insert(items);

		protected override IEnumerable<KeyValuePair<byte[], ILazyLoading<byte[]>>> Evaluate()
		{
			_dbIODevice.Reset();

			DatabaseItem dbItem;

			while (!(dbItem = _dbIODevice.ReadNext()).EndOfItems)
			{
				yield return new KeyValuePair<byte[], ILazyLoading<byte[]>>
				(
					key: dbItem.Key,
					value: new LazyLoadValue(_dbIODevice, dbItem.DataPosition)
				);
			}
		}

		public override void Dispose() => _dbIODevice.Dispose();
	}
}