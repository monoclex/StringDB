using FluentAssertions;

using Moq;

using StringDB.Querying;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.QueryingTests.cs
{
	public class SimpleDatabaseValueRequestTests
	{
		[Fact]
		public async Task OnlyOne_CallToLazyLoader_IsMade_WhenMutlipleThreads_RequestData()
		{
			var lazyLoader = new Mock<ILazyLoader<int>>();
			lazyLoader.Setup(x => x.Load()).Returns(7);

			var mres = new ManualResetEventSlim(false);
			var sdvr = new SimpleDatabaseValueRequest<int>
			(
				lazyLoader.Object,
				new SemaphoreSlim(1)
			);

			var tasks = Enumerable.Repeat(Task.Run(async () =>
			{
				mres.Wait();

				var result = await sdvr.Request()
					.ConfigureAwait(false);

				result.Should().Be(7);
			}), 1000)
				.ToArray();

			mres.Set();

			await Task.WhenAll(tasks).ConfigureAwait(false);

			lazyLoader.Verify(x => x.Load(), Times.Once());
		}
	}
}