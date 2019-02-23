using StringDB.Fluency;
using StringDB.IO;
using StringDB.Transformers;

using System.IO;

namespace StringDB
{
	public static class StringDatabase
	{
		public static IDatabase<string, string> Create()
			=> new DatabaseBuilder()
				.UseMemoryDatabase<string, string>();

		public static IDatabase<string, string> Create
		(
			Stream stream,
			bool leaveStreamOpen = false
		)
			=> Create(stream, StringDBVersions.Latest, leaveStreamOpen);

		public static IDatabase<string, string> Create
		(
			Stream stream,
			StringDBVersions version,
			bool leaveStreamOpen
		)
			=> new DatabaseBuilder()
				.UseIODatabase((builder) => builder.UseStringDB(version, stream, leaveStreamOpen))
				.ApplyStringTransformation();

		private static IDatabase<string, string> ApplyStringTransformation
		(
			this IDatabase<byte[], byte[]> database
		)
		{
			var transformer = new StringTransformer();

			return database.WithTransform(transformer, transformer);
		}
	}
}