using FluentAssertions;

using StringDB.Querying;
using StringDB.Querying.Messaging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.Querying
{
	public class PipeRequestTests : IDisposable
	{
		private readonly IMessagePipe<KeyValuePair<int, PipeRequest<int, string>>> _requestPipe;
		private readonly IMessagePipe<string> _valuePipe;
		private readonly PipeRequest<int, string> _pipeRequest;
		private readonly int _requestKey = 5;

		public PipeRequestTests()
		{
			_requestPipe = new ChannelMessagePipe<KeyValuePair<int, PipeRequest<int, string>>>();
			_valuePipe = new ChannelMessagePipe<string>();
			_pipeRequest = new PipeRequest<int, string>(_requestPipe, _valuePipe, _requestKey);
		}

		public void Dispose()
		{
			_pipeRequest.Dispose();
		}

		[Fact]
		public async Task When_RequestIsCalled_AMessageIsPutIn_TheRequestPipe()
		{
			// setup

			// this should put a message in
			var requestTask = _pipeRequest.Request();

			// create a cancellation token - if there's nothing in the pipe, it'll timeout
			var cts = new CancellationTokenSource();
			cts.CancelAfter(TimeSpan.FromSeconds(1));

			var element = await _requestPipe.Dequeue(cts.Token).ConfigureAwait(false);

			if (cts.IsCancellationRequested)
			{
				// it timesout & fails
				throw new OperationCanceledException("Request pipe took too long to dequeue.");
			}

			// ensure that the correct values were put in the message
			element.Key.Should().Be(_requestKey);
			element.Value.Should().Be(_pipeRequest);
		}

		[Fact]
		public async Task When_AValue_IsPutInTheValuePipe_AllTasksAwaitingARequest_Complete()
		{
			const string valuePutIn = "ecks dee";

			// all pipes will now request a value
			var tasks = Enumerable.Repeat<Func<Task>>(async () =>
			{
				var result = await _pipeRequest.Request().ConfigureAwait(false);

				result.Should().Be(valuePutIn);
			}, 15).Select(factory => factory()).ToArray();

			// let's put in a value
			_valuePipe.Enqueue(valuePutIn);

			// all of our tasks should complete now

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		[Fact]
		public async Task PipeRequest_DoesNotComplete_WhenNothingHappens()
		{
			// if you ever modify this, make sure to CONFIRM that this all still works
			// by changing 'var request = ...' to equal Task.FromResult(true);
			// since this is a mega hackjob D:
			// would accept a PR for some FluentAssertions thing that WORKs

			var requestTask = ((Func<Task>)(async () =>
			{
				var request = _pipeRequest.Request(); // Task.FromResult(true);

				await request.ConfigureAwait(false);

				Assert.False(true, "This shouldn't've made it this far. The dequeue completed, which is bad.");
			}))();

			// this should NOT continue
			await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(1)), requestTask)
				.ConfigureAwait(false);

			if (requestTask.IsCompleted)
			{
				throw requestTask.Exception;
			}
		}
	}
}