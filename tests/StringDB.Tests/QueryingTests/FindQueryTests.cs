using FluentAssertions;
using Moq;
using StringDB.Querying;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class FindQueryTests : IDisposable
	{
		private FindQuery<string, int> _query = new FindQuery<string, int>(str => str == "yes");

		public void Dispose() => _query.Dispose();

		[Fact]
		public void IsCancelled_Reflects_StateOf_CancellationToken()
		{
			using var cts = new CancellationTokenSource();

			_query = new FindQuery<string, int>(_ => false, cts.Token);

			_query.IsCancellationRequested
				.Should().BeFalse();

			cts.Cancel();

			_query.IsCancellationRequested
				.Should().BeTrue();
		}

		[Fact]
		public async Task ReturnsCompleted_WhenQueryItem_IsDesignedItem()
		{
			var mock = new Mock<IRequest<int>>();

			var result = await _query.Accept("yes", mock.Object).ConfigureAwait(false);
			result.Should().Be(QueryAcceptance.Completed);
		}

		[Fact]
		public async Task ReturnsNotAccepted_WhenQueryItem_IsDesignedItem()
		{
			var mock = new Mock<IRequest<int>>();

			var result = await _query.Accept("no", mock.Object).ConfigureAwait(false);
			result.Should().Be(QueryAcceptance.NotAccepted);
		}

		[Fact]
		public async Task SetsKeyAndValue_WhenProcessing()
		{
			var mock = new Mock<IRequest<int>>();
			mock.Setup(x => x.Request()).Returns(Task.FromResult(1337));

			await _query.Process("yes", mock.Object).ConfigureAwait(false);

			_query.Key
				.Should().Be("yes");

			_query.Value
				.Should().Be(1337);
		}
	}
}
