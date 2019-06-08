using FluentAssertions;

using StringDB.Querying;

using System;
using System.Threading;

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

			_query = new Query<int, int>(null, null, cts.Token);

			_query.IsCancellationRequested
				.Should().BeFalse();

			cts.Cancel();

			_query.IsCancellationRequested
				.Should().BeTrue();
		}

		[Fact]
		public void Process_InvokesDelegate_And_Parameters_ArePassed()
		{
			var invoked = false;
			var p1 = 0;
			var p2 = 0;

			_query = new Query<int, int>(null, (_1, _2) =>
			{
				p1 = _1;
				p2 = _2;

				invoked = true;
				return null;
			});

			_query.Process(13, 37);

			invoked
				.Should().BeTrue();

			p1.Should()
				.Be(13);

			p2.Should()
				.Be(37);
		}

		public void Dispose() => _query.Dispose();
	}
}