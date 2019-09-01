using FluentAssertions;

using StringDB.Querying;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.Querying
{
	public class PipeRequestManagerTests : IDisposable
	{
		private readonly PipeRequestManager<int, string> _requestManager = new PipeRequestManager<int, string>();

		[Fact]
		public async Task When_RequestCreated_AndValueRequested_NextRequestReturns()
		{
			var request = _requestManager.CreateRequest(7);

			var background = ((Func<Task>)(async () =>
			{
				var nextRequest = await _requestManager.NextRequest();

				nextRequest.RequestKey.Should().Be(7);
				nextRequest.SupplyValue("seven");
			}))();

			var result = await request.Request().ConfigureAwait(false);
			result.Should().Be("seven");

			// this should've been completed

			if (!background.IsCompleted)
			{
				throw new InvalidOperationException("background should've been completed");
			}

			// jUsT iN cAsE
			await background.ConfigureAwait(false);
		}

		[Fact]
		public async Task When_MultipleRequestsCreated_AndValuesRequested_OnlyOneNextReturn_IsCalled()
		{
			var nextWaiter = ((Func<Task>)(async () =>
			{
				var request = await _requestManager.NextRequest().ConfigureAwait(false);

				request.SupplyValue("seven");

				await _requestManager.NextRequest();

				Assert.False(true, "Expected NextRequest to not pass a second time.");
			}))();

			var request = _requestManager.CreateRequest(7);

			var awaiters = Enumerable.Repeat((Func<Task>)(async () =>
			{
				var value = await request.Request().ConfigureAwait(false);

				value.Should().Be("seven");
			}), 100)
				.Select(factory => factory())
				.ToArray();

			await Task.WhenAll(awaiters).ConfigureAwait(false);
		}

		public void Dispose()
		{
			_requestManager.Dispose();
		}
	}
}