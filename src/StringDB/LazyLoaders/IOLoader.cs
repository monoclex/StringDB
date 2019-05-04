using JetBrains.Annotations;

using StringDB.Databases;
using StringDB.IO;

namespace StringDB.LazyLoaders
{
	/// <summary>
	/// The <see cref="ILazyLoader{T}"/> used for <see cref="IODatabase"/>s.
	/// </summary>
	[PublicAPI]
	public sealed class IOLoader : ILazyLoader<byte[]>
	{
		private readonly IDatabaseIODevice _dbIODevice;
		private readonly long _position;

		public IOLoader([PublicAPI] IDatabaseIODevice dbIODevice, long position)
		{
			_dbIODevice = dbIODevice;
			_position = position;
		}

		public byte[] Load() => _dbIODevice.ReadValue(_position);
	}
}