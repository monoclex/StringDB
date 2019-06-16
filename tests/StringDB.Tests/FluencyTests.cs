using FluentAssertions;

using StringDB.Databases;
using StringDB.Fluency;
using StringDB.IO;
using StringDB.IO.Compatibility;
using StringDB.Tests.Mocks;
using StringDB.Transformers;

using System;
using System.IO;

using Xunit;

namespace StringDB.Tests
{
	/*
	 * 
	 * These aren't meant to say "ah ha yes i can expect .Method()
	 * to give me a MethodDatabase every time!" they're just here
	 * to hit that sweet :100: unit testing percent
	 * 
	 */

	public class FluencyTests
	{
		[Fact]
		public void ReturnsMemoryDatabase()
		{
			var db = new DatabaseBuilder()
				.UseMemoryDatabase<int, string>();

			db
				.Should()
				.BeOfType<MemoryDatabase<int, string>>();
		}

		[Fact]
		public void ReturnsIODatabase()
		{
			var db = new DatabaseBuilder()
				.UseIODatabase(new StoneVaultIODevice(new MemoryStream()));

			db
				.Should()
				.BeOfType<IODatabase>();
		}

		[Fact]
		public void ReturnsIODatabase_WhenUsingBuilderPattern()
		{
			var db = new DatabaseBuilder()
				.UseIODatabase(builder => builder.UseStoneVault(new MemoryStream()));

			db
				.Should()
				.BeOfType<IODatabase>();
		}

		[Fact]
		public void ReturnsTransformDatabase()
		{
			var db = new DatabaseBuilder()
				.UseMemoryDatabase<string, int>()
				.WithTransform(new MockTransformer().Reverse(), new MockTransformer());

			db.Should()
				.BeOfType<TransformDatabase<string, int, int, string>>();
		}

		[Fact]
		public void ReturnsThreadLockDatabase()
		{
			var db = new DatabaseBuilder()
				.UseMemoryDatabase<string, int>()
				.WithThreadLock();

			db.Should()
				.BeOfType<ThreadLockDatabase<string, int>>();
		}

		[Fact]
		public void ReturnsCacheDatabase()
		{
			var db = new DatabaseBuilder()
				.UseMemoryDatabase<string, int>()
				.WithCache();

			db.Should()
				.BeOfType<CacheDatabase<string, int>>();
		}

		[Fact]
		public void CreatesStringDB5_0_0LowlevelDatabaseIODevice()
		{
			var dbiod = new DatabaseIODeviceBuilder()
				.UseStringDB(StringDBVersion.v5_0_0, new MemoryStream());

			dbiod
				.Should()
				.BeOfType<DatabaseIODevice>();
		}

		[Fact]
		public void CreatesStringDB10_0_0LowlevelDatabaseIODevice()
		{
			var dbiod = new DatabaseIODeviceBuilder()
				.UseStringDB(StringDBVersion.v10_0_0, new MemoryStream());

			dbiod
				.Should()
				.BeOfType<DatabaseIODevice>();

			((DatabaseIODevice)dbiod)
				.LowLevelDatabaseIODevice
				.Should()
				.BeOfType<StringDB10_0_0LowlevelDatabaseIODevice>();
		}

		[Fact]
		public void LatestIsJust10()
		{
			var dbiod = new DatabaseIODeviceBuilder()
				.UseStringDB(StringDBVersion.Latest, new MemoryStream());

			dbiod
				.Should()
				.BeOfType<DatabaseIODevice>();

			((DatabaseIODevice)dbiod)
				.LowLevelDatabaseIODevice
				.Should()
				.BeOfType<StringDB10_0_0LowlevelDatabaseIODevice>();
		}

		[Fact]
		public void ThrowsOnInvalidStringDBCreation()
		{
			Action throws = () => new DatabaseIODeviceBuilder()
				.UseStringDB((StringDBVersion)1337, new MemoryStream());

			throws.Should()
				.ThrowExactly<NotSupportedException>();
		}

		[Fact]
		public void CreatesStoneVaultIODevice()
		{
			var dbiod = new DatabaseIODeviceBuilder()
				.UseStoneVault(new MemoryStream());

			dbiod
				.Should()
				.BeOfType<StoneVaultIODevice>();
		}

		[Fact]
		public void UseIODatabaseWithVersionAndFile()
		{
			// verify that the DBIODevice was created correctly

			var db = new DatabaseBuilder()
				.UseIODatabase(StringDBVersion.v10_0_0, "test1.db");

			db
				.Should()
				.BeOfType<IODatabase>();

			var iodb = db as IODatabase;

			iodb.DatabaseIODevice
				.Should()
				.BeOfType<DatabaseIODevice>();

			var dbiod = iodb.DatabaseIODevice as DatabaseIODevice;

			dbiod.LowLevelDatabaseIODevice
				.Should()
				.BeOfType<StringDB10_0_0LowlevelDatabaseIODevice>();
		}

		[Fact]
		public void UseStringDB()
		{
			// just make sure that down the chain we initialized a file stream
			using (var idbiod = new DatabaseIODeviceBuilder()
				.UseStringDB(StringDBVersion.v10_0_0, "test2.db"))
			{
				idbiod
					.Should()
					.BeOfType<DatabaseIODevice>();

				var dbiod = idbiod as DatabaseIODevice;

				dbiod.LowLevelDatabaseIODevice
					.Should()
					.BeOfType<StringDB10_0_0LowlevelDatabaseIODevice>();

				var ldbiod = dbiod.LowLevelDatabaseIODevice as StringDB10_0_0LowlevelDatabaseIODevice;

				ldbiod.InnerStream
					.Should()
					.BeOfType<StreamCacheMonitor>();

				var scm = ldbiod.InnerStream as StreamCacheMonitor;

				scm.InnerStream
					.Should()
					.BeOfType<FileStream>();

				var fs = scm.InnerStream as FileStream;

				fs.CanRead.Should().BeTrue();
				fs.CanWrite.Should().BeTrue();
			}
		}

		[Fact]
		public void UseKeyTransform()
		{
			using var memdb = new DatabaseBuilder()
				.UseMemoryDatabase<int, int>();

			using var transform = memdb.WithKeyTransform(IntToStringTransformer.Default);

			transform.Should().BeOfType<TransformDatabase<int, int, string, int>>();
		}

		[Fact]
		public void UseValueTransform()
		{
			using var memdb = new DatabaseBuilder()
				.UseMemoryDatabase<int, int>();

			using var transform = memdb.WithValueTransform(IntToStringTransformer.Default);

			transform.Should().BeOfType<TransformDatabase<int, int, int, string>>();
		}

		[Fact]
		public void MakeReadOnly()
		{
			using var memdb = new DatabaseBuilder()
				.UseMemoryDatabase<int, int>();

			var readOnly = memdb.AsReadOnly();

			readOnly.Should().BeOfType<ReadOnlyDatabase<int, int>>();
		}

		[Fact]
		public void MakeWriteOnly()
		{
			using var memdb = new DatabaseBuilder()
				.UseMemoryDatabase<int, int>();

			var readOnly = memdb.AsWriteOnly();

			readOnly.Should().BeOfType<WriteOnlyDatabase<int, int>>();
		}

		[Fact]
		public void WithBufferedDatabase()
		{
			using var memdb = new DatabaseBuilder()
				.UseMemoryDatabase<int, int>();

			var buffered = memdb.WithBuffer();

			buffered.Should().BeOfType<BufferedDatabase<int, int>>();
		}
	}
}