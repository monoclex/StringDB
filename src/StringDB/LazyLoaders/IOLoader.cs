using JetBrains.Annotations;

using StringDB.Databases;
using StringDB.IO;

namespace StringDB.LazyLoaders
{
	/// <summary>
	/// The <see cref="ILazyLoader{T}"/> used for <see cref="IODatabase"/>s.
	/// </summary>
	[PublicAPI]
	public struct IOLoader : ILazyLoader<byte[]>
	{
		private readonly IDatabaseIODevice _dbIODevice;
		private readonly long _position;

		/// <summary>
		/// Creates a new <see cref="IOLoader"/>.
		/// </summary>
		/// <param name="dbIODevice">The database IO device to read the value from.</param>
		/// <param name="position">The position the value is stored at.</param>
		public IOLoader
		(
			[NotNull] IDatabaseIODevice dbIODevice,
			long position
		)
		{
			_dbIODevice = dbIODevice;
			_position = position;
		}

		/// <inheritdoc />
		public byte[] Load() => _dbIODevice.ReadValue(_position);
	}
}