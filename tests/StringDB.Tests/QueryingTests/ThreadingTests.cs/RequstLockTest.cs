using FluentAssertions;

using StringDB.Querying.Threading;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests.QueryingTests.ThreadingTests.cs
{
	public class RequstLockTest
	{
		[Fact]
		public void SemaphoreSlim_GetsPassed_ToTheProperty()
		{
			var sl = new SemaphoreSlim(1);

			new RequestLock(sl)
				.SemaphoreSlim
				.Should().BeSameAs(sl);
		}

		[Fact]
		public async Task Integration()
		{
			var rl = new RequestLock(new SemaphoreSlim(1));

			bool live = true;

			var master = Task.Run(async () =>
			{
				await rl.SemaphoreSlim.WaitAsync()
					.ConfigureAwait(false);

				try
				{
					while (live)
					{
						await Task.Delay(100)
							.ConfigureAwait(false);

						await rl.AllowRequestsAsync()
							.ConfigureAwait(false);
					}
				}
				finally
				{
					rl.SemaphoreSlim.Release();
				}
			});

			var users = new List<bool>();

			Func<Task> asyncLambda = async () =>
			{
				await rl.RequestAsync()
					.ConfigureAwait(false);

				try
				{
					users.Add(true);
				}
				finally
				{
					rl.Release();
				}
			};

			var s1 = Task.Run(asyncLambda);
			var s2 = Task.Run(asyncLambda);
			var s3 = Task.Run(asyncLambda);

			await Task.WhenAll(s1, s2, s3)
				.ConfigureAwait(false);

			users.Count
				.Should().Be(3);

			// kill
			live = false;
			await master;
		}
	}
}