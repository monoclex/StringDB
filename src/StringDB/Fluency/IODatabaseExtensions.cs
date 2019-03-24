using JetBrains.Annotations;

using StringDB.Databases;
using StringDB.IO;

using System;
using System.Runtime.CompilerServices;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for an <see cref="IODatabase"/>
	/// </summary>
	[PublicAPI]
	public static class IODatabaseExtensions
	{
		/// <summary>
		/// Create a new IODatabase with the specified <see cref="IDatabaseIODevice"/>.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="databaseIODevice">The <see cref="IDatabaseIODevice"/> to pass to the IODatabase.</param>
		/// <returns>An IODatabase.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<byte[], byte[]> UseIODatabase
		(
			[CanBeNull] this DatabaseBuilder builder,
			[NotNull] IDatabaseIODevice databaseIODevice
		)
			=> new IODatabase(databaseIODevice);

		/// <summary>
		/// Create a new IODatabase and allows for fluent usage to create an IDatabaseIODevice.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="databaseIODevice">A delegate that allows for fluent building of an IDatabaseIODevice.</param>
		/// <returns>An IODatabase.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<byte[], byte[]> UseIODatabase
		(
			[CanBeNull] this DatabaseBuilder builder,
			[NotNull] Func<DatabaseIODeviceBuilder, IDatabaseIODevice> databaseIODevice
		)
			=> builder.UseIODatabase(databaseIODevice(new DatabaseIODeviceBuilder()));

		/// <summary>
		/// Creates a new IODatabase using StringDB.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="version">The version of StringDB to use.</param>
		/// <param name="file">The file to read from.</param>
		/// <returns>An <see cref="IODatabase"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<byte[], byte[]> UseIODatabase
		(
			[CanBeNull] this DatabaseBuilder builder,
			StringDBVersions version,
			[NotNull] string file
		)
			=> builder.UseIODatabase(databaseIODeviceBuilder => databaseIODeviceBuilder.UseStringDB(version, file));
	}
}