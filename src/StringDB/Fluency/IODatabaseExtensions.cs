using StringDB.Databases;
using StringDB.IO;

namespace StringDB.Fluency
{
	public static class IODatabaseExtensions
	{
		public static IDatabase<byte[], byte[]> UseIODatabaseProvider
		(
			this DatabaseBuilder builder,
			IDatabaseIODevice databaseIODevice
		)
			=> new IODatabase(databaseIODevice);
	}
}