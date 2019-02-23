using StringDB.Fluency;
using StringDB.IO;
using StringDB.Transformers;

using System.IO;

namespace StringDB
{
	/// <summary>
	/// A simple utility class to allow for very easy creation of string databases.
	/// </summary>
	public static class StringDatabase
	{
		/// <summary>
		/// Creates a string database entirely in memory.
		/// </summary>
		/// <returns>A string database, located in memory.</returns>
		public static IDatabase<string, string> Create()
			=> new DatabaseBuilder()
				.UseMemoryDatabase<string, string>();

		/// <summary>
		/// Creates a string database that saves to a stream (file).
		/// </summary>
		/// <param name="stream">The stream to write/read data to/from.</param>
		/// <param name="leaveStreamOpen">If the stream should be left open when the database is getting disposed.</param>
		/// <returns>An IODatabase with a transform, using the latest version of StringDB.</returns>
		public static IDatabase<string, string> Create
		(
			Stream stream,
			bool leaveStreamOpen = false
		)
			=> Create(stream, StringDBVersions.Latest, leaveStreamOpen);

		/// <summary>
		/// Creates a string database that saves to a stream, and specify the version.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="version">The version of StringDB.</param>
		/// <param name="leaveStreamOpen">If the stream should be left open when the database is getting disposed.</param>
		/// <returns>An IODatabase with a transform, using the specified version of StringDB.</returns>
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