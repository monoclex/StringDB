using FluentAssertions;
using StringDB.Querying;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.QueryingTests
{
	// how do i even test this :hrmf:
	// i'll just test the controller bit of it

	public class ParallelAsyncTests
	{
		[Fact]
		public async Task SetResult_ReturnsResult()
		{
			var result = await ParallelAsync.ForEachAsync<int, int>(new int[] { 0 }, async (item, controller) =>
			{
				controller.ProvideResult(item);
			}).ConfigureAwait(false);

			result.Should().Be(0);
		}

		[Fact]
		public async Task Stop_Stops()
		{
			bool eval1 = false;
			bool eval2 = false;

			var result = await ParallelAsync.ForEachAsync<Action, int>(new Action[]
			{
				() => eval1 = true,
				() => eval2 = true
			}, async (item, controller) =>
			{
				item();

				controller.ProvideResult(0);
				controller.Stop();
			}, 1).ConfigureAwait(false);

			result.Should().Be(0);

			eval1.Should().BeTrue();
			eval2.Should().BeFalse();
		}
	}
}
