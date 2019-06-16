using FluentAssertions;

using Moq;

using StringDB.Querying;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.QueryingTests.cs
{
	public class SimpleDatabaseValueRequestTests
	{
		private readonly Mock<ILazyLoader<int>> _loader;
		private readonly SemaphoreSlim _lock;
		private readonly SimpleDatabaseValueRequest<int> _sdvr;

		public SimpleDatabaseValueRequestTests()
		{
			_loader = new Mock<ILazyLoader<int>>();
			_loader.Setup(x => x.Load()).Returns(7);

			_lock = new SemaphoreSlim(1);

			_sdvr = new SimpleDatabaseValueRequest<int>
			(
				_loader.Object,
				_lock
			);
		}

		[Fact]
		public async Task OnlyOne_CallToLazyLoader_IsMade_WhenMutlipleThreads_RequestData()
		{
			var mres = new ManualResetEventSlim(false);

			var tasks = Enumerable.Repeat(Task.Run(async () =>
			{
				mres.Wait();

				var result = await _sdvr.Request()
					.ConfigureAwait(false);

				result.Should().Be(7);
			}), 1000)
				.ToArray();

			mres.Set();

			await Task.WhenAll(tasks).ConfigureAwait(false);

			_loader.Verify(x => x.Load(), Times.Once());
		}

		[Fact]
		public async Task RepeatCalls_GetCached()
		{
			var result = await _sdvr.Request().ConfigureAwait(false);
			result.Should().Be(7);

			result = await _sdvr.Request().ConfigureAwait(false);
			result.Should().Be(7);

			_loader.Verify(x => x.Load(), Times.Once());
		}

		// lol
		[Fact]
		public void Disposing_DoesntFail()
		{
			_sdvr.Dispose();
		}
	}
}