using JetBrains.Annotations;

using StringDB.IO;
using StringDB.IO.Compatibility;

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="DatabaseIODeviceBuilder"/>
	/// </summary>
	[PublicAPI]
	public static class DatabaseIODeviceBuilderExtensions
	{
		/// <summary>
		/// Use a <see cref="StoneVaultIODevice"/>.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="stream">The stream to use.</param>
		/// <param name="leaveStreamOpen">If the stream should be left open after disposing of the <see cref="IDatabaseIODevice"/>.</param>
		/// <returns>A <see cref="StoneVaultIODevice"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabaseIODevice UseStoneVault
		(
			[CanBeNull] this DatabaseIODeviceBuilder builder,
			[NotNull] Stream stream,
			bool leaveStreamOpen = false
		)
			=> new StoneVaultIODevice(stream, leaveStreamOpen);

		/// <summary>
		/// Use StringDB from a file.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="version">The version of StringDB to use.</param>
		/// <param name="file">The file to open.</param>
		/// <returns>An <see cref="IDatabaseIODevice"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabaseIODevice UseStringDB
		(
			[CanBeNull] this DatabaseIODeviceBuilder builder,
			StringDBVersion version,
			[NotNull] string file
		)
			=> builder.UseStringDB(version, file, NoByteBuffer.Read);

		/// <summary>
		/// Use StringDB from a file.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="version">The version of StringDB to use.</param>
		/// <param name="file">The file to open.</param>
		/// <param name="buffer">The kind of byte[] reading technique to use.</param>
		/// <returns>An <see cref="IDatabaseIODevice"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabaseIODevice UseStringDB
		(
			[CanBeNull] this DatabaseIODeviceBuilder builder,
			StringDBVersion version,
			[NotNull] string file,
			[NotNull] Func<BinaryReader, int, byte[]> buffer
		)
			=> builder
				.UseStringDB
				(
					version,
					File.Open
					(
						file,
						FileMode.OpenOrCreate,
						FileAccess.ReadWrite
					),
					buffer
				);

		/// <summary>
		/// Use a StringDB IDatabaseIODevice
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="version">The version of StringDB to accomodate.</param>
		/// <param name="stream">The stream to use.</param>
		/// <param name="leaveStreamOpen">If the stream should be left open after disposing of the <see cref="IDatabaseIODevice"/>.</param>
		/// <returns>An <see cref="IDatabaseIODevice"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabaseIODevice UseStringDB
		(
			[CanBeNull] this DatabaseIODeviceBuilder builder,
			StringDBVersion version,
			[NotNull] Stream stream,
			bool leaveStreamOpen = false
		)
			=> builder.UseStringDB(version, stream, NoByteBuffer.Read, leaveStreamOpen);

		/// <summary>
		/// Use a StringDB IDatabaseIODevice
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="version">The version of StringDB to accomodate.</param>
		/// <param name="stream">The stream to use.</param>
		/// <param name="buffer">The kind of byte[] reading technique to use.</param>
		/// <param name="leaveStreamOpen">If the stream should be left open after disposing of the <see cref="IDatabaseIODevice"/>.</param>
		/// <returns>An <see cref="IDatabaseIODevice"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabaseIODevice UseStringDB
		(
			[CanBeNull] this DatabaseIODeviceBuilder builder,
			StringDBVersion version,
			[NotNull] Stream stream,
			[NotNull] Func<BinaryReader, int, byte[]> buffer,
			bool leaveStreamOpen = false
		)
			=> new DatabaseIODevice
			(
				version.UseVersion(stream, buffer, leaveStreamOpen)
			);

		[NotNull]
		private static ILowlevelDatabaseIODevice UseVersion
		(
			this StringDBVersion version,
			[NotNull] Stream stream,
			Func<BinaryReader, int, byte[]> buffer,
			bool leaveStreamOpen = false
		)
		{
			switch (version)
			{
				case StringDBVersion.v5_0_0: return new StringDB5_0_0LowlevelDatabaseIODevice(stream, buffer, leaveStreamOpen);
				case StringDBVersion.v10_0_0: return new StringDB10_0_0LowlevelDatabaseIODevice(stream, buffer, leaveStreamOpen);
				default: throw new NotSupportedException($"Didn't expect a {version}");
			}
		}
	}
}