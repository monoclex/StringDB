using FluentAssertions;

using StringDB.Fluency;
using StringDB.Querying;
using StringDB.Querying.Queries;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class QueryManagerTests : IDisposable
	{
		private readonly IDatabase<string, string> _db;
		private readonly IQueryManager<string, string> _query;

		public QueryManagerTests()
		{
			_db = new DatabaseBuilder()
				.UseMemoryDatabase<string, string>();

			_db.Insert("key1", "value1");
			_db.Insert("key2", "value2");
			_db.Insert("key3", "value3");

			_query = new QueryManager<string, string>(_db);
		}

		public void Dispose() => _query.Dispose();

		[Fact]
		public async Task Integration_Works()
		{
			// let's hope nothing fails :D

			int count = 0;

			Parallel.For(0, 100, async i =>
			{
				await _query.ExecuteQuery(new Query<string, string>(async (a, b) =>
					QueryAcceptance.Continue
				)).ConfigureAwait(false);

				await _query.ExecuteQuery(new WriteQuery<string, string>(db =>
					db.Insert("key4", "value4")
				)).ConfigureAwait(false);

				await _query.ExecuteQuery(new Query<string, string>(async (a, b) =>
					QueryAcceptance.Continue
				)).ConfigureAwait(false);

				Interlocked.Increment(ref count);
			});

			// i'm lazy lmao

			while (count != 100)
			{
				await Task.Delay(100);
			}

			_db.Count()
				.Should().Be(3 + 100);
		}
	}
}