using FluentAssertions;
using Moq;
using StringDB.Querying;
using StringDB.Querying.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class QueryPoolTests : IDisposable
	{
		private readonly QueryPool<string, int> _queryPool;
		private readonly Mock<IRequest<int>> _queryRequest;

		private readonly Mock<IQuery<string, int>> _query1;
		private readonly Mock<IQuery<string, int>> _query2;
		private readonly Mock<IQuery<string, int>> _query3;

		private readonly QueryItem<string, int> _qItem1;
		private readonly QueryItem<string, int> _qItem2;
		private readonly QueryItem<string, int> _qItem3;

		public QueryPoolTests()
		{
			_queryPool = new QueryPool<string, int>();
			_queryRequest = new Mock<IRequest<int>>(MockBehavior.Strict);
			_queryRequest.Setup(x => x.Request()).Returns(Task.FromResult(7));

			_query1 = new Mock<IQuery<string, int>>(MockBehavior.Strict);
			_query2 = new Mock<IQuery<string, int>>(MockBehavior.Strict);
			_query3 = new Mock<IQuery<string, int>>(MockBehavior.Strict);

			_qItem1 = new QueryItem<string, int>
			{
				CompletionSource = new TaskCompletionSource<bool>(),
				Query = _query1.Object,
				Index = 0
			};

			_qItem2 = new QueryItem<string, int>
			{
				CompletionSource = new TaskCompletionSource<bool>(),
				Query = _query2.Object,
				Index = 1
			};

			_qItem3 = new QueryItem<string, int>
			{
				CompletionSource = new TaskCompletionSource<bool>(),
				Query = _query3.Object,
				Index = 2
			};
		}

		[Fact]
		public async Task Executes_AllQueries()
		{
			await _queryPool.Append(_qItem1);
			await _queryPool.Append(_qItem2);
			await _queryPool.Append(_qItem3);

			_query1.Setup(x => x.Accept("a", It.IsAny<IRequest<int>>())).Returns(Task.FromResult(QueryAcceptance.NotAccepted)).Verifiable();
			_query3.Setup(x => x.Accept("a", It.IsAny<IRequest<int>>())).Returns(Task.FromResult(QueryAcceptance.Completed)).Verifiable();

			_query3.Setup(x => x.Process("a", It.IsAny<IRequest<int>>())).Returns(Task.CompletedTask).Verifiable();

			// execute it
			await _queryPool.ExecuteQueries(1, "a", _queryRequest.Object)
				.ConfigureAwait(false);

			// now 1 and 2 shouldn't exist
			var queries = await _queryPool.CurrentQueries()
				.ConfigureAwait(false);

			// TODO: verify that it was actually 1 and 2 that died
			queries.Count
				.Should().Be(1);

			_query1.VerifyAll();
			_query2.VerifyAll();
			_query3.VerifyAll();
		}

		public void Dispose()
		{
			_queryPool.Dispose();
		}
	}
}
