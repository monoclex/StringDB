using FluentAssertions;
using StringDB.Querying;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.Querying
{
	public class BackgroundTaskTests
	{
		[Fact]
		public async Task BackgroundTask_Works_AndDies()
		{
			int counter = int.MinValue;

			var bg = new BackgroundTask(async (cts) =>
			{
				await Task.Yield();

				while (!cts.IsCancellationRequested)
				{
					counter++;
				}
			});

			await Task.Delay(1000).ConfigureAwait(false);

			bg.Dispose();

			// this should complete fairly instantly
			await bg.Task;

			counter.Should().BeGreaterThan(int.MinValue);
		}

		[Fact]
		public async Task BackgroundTask_WithValue_Works_AndDies()
		{
			var bg = new BackgroundTask<int>(async (cts) =>
			{
				await Task.Yield();

				var counter = int.MinValue;

				while (!cts.IsCancellationRequested)
				{
					counter++;
				}

				return counter;
			});

			await Task.Delay(1000).ConfigureAwait(false);

			bg.Dispose();

			// this should complete fairly instantly
			var counter = await bg.Task;

			counter.Should().BeGreaterThan(int.MinValue);
		}
	}
}
