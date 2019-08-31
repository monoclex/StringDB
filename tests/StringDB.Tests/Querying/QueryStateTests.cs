using FluentAssertions;
using Moq;
using StringDB.Querying;
using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.Querying
{
	public class QueryStateTests : IDisposable
	{
		private IMessagePipe<KeyValuePair<int, IRequest<string>>> _pipe;
		private IQuery<int, string> _query;
		private QueryState<int, string> _state;

		[Fact]
		public async Task Will_RequestEntries_And_ProcessThem_Synchronously()
		{
			// setup
			_pipe = new SimpleMessagePipe<KeyValuePair<int, IRequest<string>>>();
			_pipe.Enqueue(KeyValuePair.Create(0, (IRequest<string>)null));
			_pipe.Enqueue(KeyValuePair.Create(1, (IRequest<string>)null));
			_pipe.Enqueue(KeyValuePair.Create(2, (IRequest<string>)null));

			var queryMock = new Mock<IQuery<int, string>>(MockBehavior.Strict);

			queryMock.SetupGet(mock => mock.CancellationToken)
				.Returns(default(CancellationToken))
				.Verifiable();

			// we want it to process 1
			queryMock.Setup(mock => mock.Process(0, null))
				.Returns(Task.FromResult(QueryAcceptance.Continue))
				.Verifiable();

			// then we dont want it to process any more
			queryMock.Setup(mock => mock.Process(1, null))
				.Returns(Task.FromResult(QueryAcceptance.Completed))
				.Verifiable();

			// so a third call is non existent and shouldn't be hit

			_query = queryMock.Object;
			_state = new QueryState<int, string>(_query, _pipe);

			// act

			await _state.Run().ConfigureAwait(false);

			// assert

			queryMock.VerifyAll();

			// should be one item left in the pipe
			var stop = new CancellationTokenSource();
			stop.CancelAfter(TimeSpan.FromSeconds(1));

			var dequeued = await _pipe.Dequeue().ConfigureAwait(false);

			// the one item left should be the (2, null)
			dequeued.Key.Should().Be(2);
		}

		public void Dispose()
		{
			// _query.Dispose();
			_pipe.Dispose();
		}
	}
}
