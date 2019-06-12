using FluentAssertions;

using Moq;

using StringDB.Querying;
using StringDB.Querying.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class QueryTests : IDisposable
	{
		private IQuery<int, int> _query;

		[Fact]
		public void IsCancelled_Reflects_StateOf_CancellationToken()
		{
			var cts = new CancellationTokenSource();

			_query = new Query<int, int>(null, cts.Token);

			_query.IsCancellationRequested
				.Should().BeFalse();

			cts.Cancel();

			_query.IsCancellationRequested
				.Should().BeTrue();
		}

		[Fact]
		public void Query_InvokesDelegate_And_Parameters_ArePassed()
		{
			var mock = new Mock<IRequest<int>>();
			mock.Setup(x => x.Request()).Returns(() => Task.FromResult(37));

			var invoked = false;
			int p1 = default;
			IRequest<int> p2 = default;

			_query = new Query<int, int>((_1, _2) =>
			{
				p1 = _1;
				p2 = _2;

				invoked = true;
				return Task.FromResult(QueryAcceptance.Completed);
			});

			_query.Process(13, mock.Object)
				.GetAwaiter()
				.GetResult()
				.Should()
				.Be(QueryAcceptance.Completed);

			invoked
				.Should().BeTrue();

			p1.Should()
				.Be(13);

			p2.Request()
				.GetAwaiter()
				.GetResult()
				.Should()
				.Be(37);
		}

		[Fact]
		public void Process_InvokesDelegate_And_Parameters_ArePassed()
		{
			var mock = new Mock<IRequest<int>>();
			mock.Setup(x => x.Request()).Returns(() => Task.FromResult(37));

			var invoked = false;
			int p1 = default;
			IRequest<int> p2 = default;

			_query = new Query<int, int>((_1, _2) =>
			{
				p1 = _1;
				p2 = _2;

				invoked = true;
				return Task.FromResult(QueryAcceptance.Continue);
			});

			_query.Process(13, mock.Object);

			invoked
				.Should().BeTrue();

			p1.Should()
				.Be(13);

			p2.Request()
				.GetAwaiter()
				.GetResult()
				.Should()
				.Be(37);
		}

		public void Dispose() => _query.Dispose();
	}
}