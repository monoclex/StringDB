using JetBrains.Annotations;

using StringDB.Databases;
using StringDB.IO;

using System;
using System.IO;
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
			=> builder.UseIODatabase(databaseIODevice, out _);

		/// <summary>
		/// Create a new IODatabase with the specified <see cref="IDatabaseIODevice"/>,
		/// and has an out parameter to specify the optimal token source.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="databaseIODevice">The <see cref="IDatabaseIODevice"/> to pass to the IODatabase.</param>
		/// <param name="optimalTokenSource">The <see cref="IOptimalTokenSource"/> to use for optimal enumeration.</param>
		/// <returns>An IODatabase.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<byte[], byte[]> UseIODatabase
		(
			[CanBeNull] this DatabaseBuilder builder,
			[NotNull] IDatabaseIODevice databaseIODevice,
			[NotNull] out IOptimalTokenSource optimalTokenSource
		)
		{
			var iodb = new IODatabase(databaseIODevice);
			optimalTokenSource = iodb.DatabaseIODevice.OptimalTokenSource;
			return iodb;
		}

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
			=> builder.UseIODatabase(databaseIODevice, out _);

		/// <summary>
		/// Create a new IODatabase and allows for fluent usage to create an IDatabaseIODevice.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="databaseIODevice">A delegate that allows for fluent building of an IDatabaseIODevice.</param>
		/// <param name="optimalTokenSource">The <see cref="IOptimalTokenSource"/> to use for optimal enumeration.</param>
		/// <returns>An IODatabase.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<byte[], byte[]> UseIODatabase
		(
			[CanBeNull] this DatabaseBuilder builder,
			[NotNull] Func<DatabaseIODeviceBuilder, IDatabaseIODevice> databaseIODevice,
			[NotNull] out IOptimalTokenSource optimalTokenSource
		)
			=> builder.UseIODatabase(databaseIODevice(new DatabaseIODeviceBuilder()), out optimalTokenSource);

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
			StringDBVersion version,
			[NotNull] string file
		)
			=> builder.UseIODatabase(version, file, out _);

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
			StringDBVersion version,
			[NotNull] string file,
			[NotNull] out IOptimalTokenSource optimalTokenSource
		)
			=> builder.UseIODatabase(databaseIODeviceBuilder => databaseIODeviceBuilder.UseStringDB(version, file), out optimalTokenSource);

		public static IDatabase<byte[], byte[]> UseIODatabase
		(
			this DatabaseBuilder builder,
			IODatabaseOptions options,
			[NotNull] out IOptimalTokenSource optimalTokenSource
		)
			=> builder.UseIODatabase(ioDeviceBuilder =>
			{
				var buffer =
					options.UseByteBuffer
					? (Func<BinaryReader, int, byte[]>)(new ByteBuffer().Read)
					: NoByteBuffer.Read;

				IDatabaseIODevice device = default;

				if (options.FileName == default)
				{
					device = ioDeviceBuilder.UseStringDB
					(
						version: options.Version,
						stream: options.Stream,
						buffer: buffer,
						leaveStreamOpen: options.LeaveStreamOpen
					);
				}
				else
				{
					device = ioDeviceBuilder.UseStringDB
					(
						version: options.Version,
						file: options.FileName,
						buffer: buffer,
						leaveStreamOpen: options.LeaveStreamOpen
					);
				}

				return device;
			}, out optimalTokenSource);

	}
}