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
			StringDBVersions version,
			[NotNull] string file
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
					)
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
			StringDBVersions version,
			[NotNull] Stream stream,
			bool leaveStreamOpen = false
		)
			=> new DatabaseIODevice
			(
				version.UseVersion(stream, leaveStreamOpen)
			);

		[NotNull]
		private static ILowlevelDatabaseIODevice UseVersion
		(
			this StringDBVersions version,
			[NotNull] Stream stream,
			bool leaveStreamOpen = false
		)
		{
			switch (version)
			{
				case StringDBVersions.v5_0_0: return new StringDB5_0_0LowlevelDatabaseIODevice(stream, leaveStreamOpen);
				case StringDBVersions.v10_0_0: return new StringDB10_0_0LowlevelDatabaseIODevice(stream, leaveStreamOpen);
				default: throw new NotSupportedException($"Didn't expect a {version}");
			}
		}
	}
}