using FluentAssertions;

using StringDB.Databases;
using StringDB.Querying;

using System;

using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class WriteQueryTests : IDisposable
	{
		private readonly IDatabase<int, int> _db = new MemoryDatabase<int, int>();

		[Fact]
		public void Executes_DelegateInConstructor_When_Invoked()
		{
			var invoked = false;

			using var wQuery = new WriteQuery<int, int>(_ => invoked = true);
			wQuery.Execute(_db);

			invoked.Should()
				.BeTrue();
		}

		[Fact]
		public void DatabaseIsPassedToDelegate_When_Invoked()
		{
			IDatabase<int, int> passedDb = default;

			using var wQuery = new WriteQuery<int, int>(db => passedDb = db);
			wQuery.Execute(_db);

			passedDb.Should()
				.BeEquivalentTo(_db);
		}

		public void Dispose() => _db.Dispose();
	}
}