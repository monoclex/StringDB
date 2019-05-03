using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

namespace StringDB.Tests
{
	public class IntegrationTests : IDisposable
	{
		private readonly MemoryStream _ms;
		private readonly Func<IDatabase<string, string>> _openDb;
		private readonly Action<IDatabase<string, string>> _insert;

		public IntegrationTests()
		{
			_ms = new MemoryStream();

			_openDb = () =>
			{
				_ms.Position = 0;
				return StringDatabase.Create(_ms, true);
			};

			_insert = db =>
			{
				db.Insert("a", "d");
				db.Insert("b", "e");
				db.Insert("c", "f");
			};
		}

		public void Dispose() => _ms.Dispose();

		[Fact]
		public void IntegrationTest()
		{
			using (var db = _openDb())
			{
				_insert(db);
			}

			using (var db = _openDb())
			{
				_insert(db);

				db.EnumerateAggressively(3)
					.Should().BeEquivalentTo(new[]
					{
							new KeyValuePair<string, string>("test", "value"),
							new KeyValuePair<string, string>("test", "value2"),
							new KeyValuePair<string, string>("test", "value3"),
					});
			}
		}
	}
}