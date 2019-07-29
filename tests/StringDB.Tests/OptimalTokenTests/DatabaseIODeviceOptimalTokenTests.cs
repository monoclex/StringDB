using FluentAssertions;

using StringDB.Fluency;
using StringDB.IO;
using StringDB.IO.Compatibility;
using StringDB.Transformers;

using System.Collections.Generic;
using System.IO;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests that ensure the optimal token gets called when it needs to be called.
	/// </summary>
	public class DatabaseIODeviceOptimalTokenTests
	{
		private readonly OptimalTokenSource _token;
		private readonly MemoryStream _ms;
		private readonly StringDB10_0_0LowlevelDatabaseIODevice _lldbiod;
		private readonly DatabaseIODevice _dbiod;

		public DatabaseIODeviceOptimalTokenTests()
		{
			_token = new OptimalTokenSource();
			_ms = new MemoryStream();

			// setup a db
			using (var _db = new DatabaseBuilder()
				.UseIODatabase(builder => builder.UseStringDB(StringDBVersion.v10_0_0, _ms, true))
				.WithTransform(StringTransformer.Default, StringTransformer.Default))
			{
				_db.InsertRange(KeyValuePair.Create("one key", "one value"), KeyValuePair.Create("two key", "two value"));
				_db.Insert("key", "value");
				_db.Insert("another key", "another value");
			}

			_ms.Position = 0;

			_lldbiod = new StringDB10_0_0LowlevelDatabaseIODevice(_ms, NoByteBuffer.Read, false);
			_dbiod = new DatabaseIODevice(_lldbiod, _token);
		}

		[Fact]
		public void TriggersOptimalTokenOnJump()
		{
			FalseToken();
			FalseToken();
			TrueToken();
			TrueToken();

			void FalseToken()
			{
				_dbiod.ReadNext();
				_token.OptimalReadingTime.Should().BeFalse();
			}

			void TrueToken()
			{
				_dbiod.ReadNext();
				_token.OptimalReadingTime.Should().BeTrue();
			}
		}
	}
}