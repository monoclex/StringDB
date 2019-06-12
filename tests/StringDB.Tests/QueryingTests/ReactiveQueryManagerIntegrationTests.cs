using FluentAssertions;

using StringDB.Fluency;
using StringDB.IO;
using StringDB.Querying;
using StringDB.Querying.Threading;
using StringDB.Transformers;

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class ReactiveQueryManagerIntegrationTests
	{
		private readonly SemaphoreSlim _requestLock;
		private readonly MemoryStream _ms;
		private readonly IDatabase<string, string> _db;
		private readonly ReactiveQueryManager<string, string> _qm;

		public ReactiveQueryManagerIntegrationTests()
		{
			_requestLock = new SemaphoreSlim(1);
			_ms = new MemoryStream();
			_db = new DatabaseBuilder()
				.UseIODatabase(builder => builder.UseStringDB(StringDBVersion.Latest, _ms, true))
				.WithTransform(StringTransformer.Default, StringTransformer.Default);
			_qm = new ReactiveQueryManager<string, string>(DatabaseEnumerable.MakeTrainEnumerable
			(
				_db,
				lazyLoader => new SimpleDatabaseValueRequest<string>(lazyLoader, _requestLock),
				_requestLock
			));

			// seed the db
			for (var i = 0; i < 10_000; i++)
			{
				_db.Insert($"{i}", $"{i}");
			}
		}

		[Fact]
		public async Task Find9_999()
		{
			var tasks = Enumerable.Repeat(Task.Run(async () =>
			{
				var kvp = await _qm.Find(s => s == "9999")
					.ConfigureAwait(false) ?? throw new Exception("Unexpected null kvp");

				kvp.Key.Should().Be("9999");
				kvp.Value.Should().Be("9999");
			}), 10_000);

			await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
		}

		[Fact]
		public async Task SearchFor9999()
		{
			var kvp = await _qm.Find(s => s == "9999")
				.ConfigureAwait(false) ?? throw new Exception("Unexpected null kvp");

			kvp.Key.Should().Be("9999");
			kvp.Value.Should().Be("9999");
		}
	}
}