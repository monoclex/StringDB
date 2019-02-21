using StringDB.Databases;
using StringDB.IO;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for an <see cref="IODatabase"/>
	/// </summary>
	public static class IODatabaseExtensions
	{
		/// <summary>
		/// Create a new IODatabase with the specified <see cref="IDatabaseIODevice"/>.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="databaseIODevice">The <see cref="IDatabaseIODevice"/> to pass to the IODatabase.</param>
		/// <returns>An IODatabase.</returns>
		public static IDatabase<byte[], byte[]> UseIODatabaseProvider
		(
			this DatabaseBuilder builder,
			IDatabaseIODevice databaseIODevice
		)
			=> new IODatabase(databaseIODevice);
	}
}