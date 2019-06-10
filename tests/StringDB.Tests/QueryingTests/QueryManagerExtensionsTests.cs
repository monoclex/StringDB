using FluentAssertions;

using Moq;

using StringDB.Querying;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class QueryManagerExtensionsTests
	{
		private Mock<IQueryManager<string, int>> _queryManager = new Mock<IQueryManager<string, int>>();

		[Fact]
		public async Task CancellationToken_IsPassed_ToQueryManager()
		{
			var cancellationRequestedShouldBe = false;

			var request = new Mock<IRequest<int>>();
			request.Setup(x => x.Request()).Returns(Task.FromResult(1));

			_queryManager.Setup(x => x.ExecuteQuery(It.IsAny<IQuery<string, int>>()))
				.Returns<IQuery<string, int>>(async query =>
				{
					query.IsCancellationRequested
						.Should()
						.Be(cancellationRequestedShouldBe);

					await query.Accept("yes", request.Object).ConfigureAwait(false);
					await query.Process("a", request.Object).ConfigureAwait(false);

					return true;
				})
				.Verifiable();

			var cts = new CancellationTokenSource();

			var kvp = await _queryManager.Object
				.Find(str => str == "yes", cts.Token)
				.ConfigureAwait(false);

			kvp.Should()
				.Be(new KeyValuePair<string, int>("a", 1));

			_queryManager.Verify();
		}

		public async Task ExecutingQuery_IsFalse_ReturnsNull()
		{
			_queryManager.Setup(x => x.ExecuteQuery(It.IsAny<IQuery<string, int>>()))
				.Returns<IQuery<string, int>>(async query => false)
				.Verifiable();

			var result = await _queryManager.Object
				.Find(x => x == "str");

			result.Should().BeNull();

			_queryManager.Verify();
		}
	}
}