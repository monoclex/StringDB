using JetBrains.Annotations;

using StringDB.IO;

using System.Collections.Generic;

namespace StringDB.Databases
{
	/// <inheritdoc />
	/// <summary>
	/// A database for IO-based operations, using an IDatabaseIODevice.
	/// </summary>
	[PublicAPI]
	public sealed class IODatabase : BaseDatabase<byte[], byte[]>
	{
		private sealed class IOLazyLoader : ILazyLoader<byte[]>
		{
			private readonly IDatabaseIODevice _dbIODevice;
			private readonly long _position;

			public IOLazyLoader([PublicAPI] IDatabaseIODevice dbIODevice, long position)
			{
				_dbIODevice = dbIODevice;
				_position = position;
			}

			public byte[] Load() => _dbIODevice.ReadValue(_position);
		}

		/// <summary>
		/// The DatabaseIODevice in use.
		/// </summary>
		public IDatabaseIODevice DatabaseIODevice { get; }

		/// <summary>
		/// Create an IODatabase with the IDatabaseIODevice specified.
		/// </summary>
		/// <param name="dbIODevice">The DatabaseIODevice to use.</param>
		public IODatabase([NotNull] IDatabaseIODevice dbIODevice)
			: this(dbIODevice, EqualityComparer<byte[]>.Default)
		{
		}

		/// <summary>
		/// Create an IODatabase with the IDatabaseIODevice specified.
		/// </summary>
		/// <param name="dbIODevice">The DatabaseIODevice to use under the hood.</param>
		/// <param name="comparer">The equality comparer to use for the key.</param>
		public IODatabase([NotNull] IDatabaseIODevice dbIODevice, [NotNull] EqualityComparer<byte[]> comparer)
			: base(comparer)
			=> DatabaseIODevice = dbIODevice;

		/// <inheritdoc />
		public override void InsertRange(params KeyValuePair<byte[], byte[]>[] items) => DatabaseIODevice.Insert(items);

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<byte[], ILazyLoader<byte[]>>> Evaluate()
		{
			DatabaseIODevice.Reset();

			DatabaseItem dbItem;

			while (!(dbItem = DatabaseIODevice.ReadNext()).EndOfItems)
			{
				yield return new KeyValuePair<byte[], ILazyLoader<byte[]>>
				(
					key: dbItem.Key,
					value: new IOLazyLoader(DatabaseIODevice, dbItem.DataPosition)
				);
			}
		}

		/// <inheritdoc />
		public override void Dispose() => DatabaseIODevice.Dispose();
	}
}