using StringDB.IO;
using StringDB.IO.Compatibility;

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace StringDB.Fluency
{
	/// <summary>
	/// A class that allows the extensive usage of extensions to create DatabaseIODevices.
	/// </summary>
	public sealed class DatabaseIODeviceBuilder
	{
	}

	public static class DatabaseIODeviceBuilderExtensions
	{
		/// <summary>
		/// Use a <see cref="StoneVaultIODevice"/>.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="stream">The stream to use.</param>
		/// <param name="leaveStreamOpen">If the stream should be left open after disposing of the <see cref="IDatabaseIODevice"/>.</param>
		/// <returns>A <see cref="StoneVaultIODevice"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabaseIODevice UseStoneVault
		(
			this DatabaseIODeviceBuilder builder,
			Stream stream,
			bool leaveStreamOpen = false
		)
			=> new StoneVaultIODevice(stream, leaveStreamOpen);

		/// <summary>
		/// Use a StringDB IDatabaseIODevice
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="version">The version of StringDB to accomodate.</param>
		/// <param name="stream">The stream to use.</param>
		/// <param name="leaveStreamOpen">If the stream should be left open after disposing of the <see cref="IDatabaseIODevice"/>.</param>
		/// <returns>An <see cref="IDatabaseIODevice"/></returns>
		public static IDatabaseIODevice UseStringDB
		(
			this DatabaseIODeviceBuilder builder,
			StringDBVersions version,
			Stream stream,
			bool leaveStreamOpen = false
		)
			=> new DatabaseIODevice
			(
				version.UseVersion(stream, leaveStreamOpen)
			);

		private static ILowlevelDatabaseIODevice UseVersion
		(
			this StringDBVersions version,
			Stream stream,
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