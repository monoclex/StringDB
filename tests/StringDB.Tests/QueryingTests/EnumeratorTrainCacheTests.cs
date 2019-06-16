using FluentAssertions;
using StringDB.Querying;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class EnumeratorTrainCacheTests
	{
		private readonly EnumeratorTrainCache<int> _cache;

		public EnumeratorTrainCacheTests()
		{
			_cache = new EnumeratorTrainCache<int>();
		}

		[Fact]
		public async Task ParticipantCount_HasCorrectCount_AfterManyConcurrentChanges()
		{
			var mres = new ManualResetEventSlim();

			var tasks = new List<Task>();

			const int addAmount = 15_000;
			const int subAmount = 10_000;

			for (var add = 0; add < addAmount; add++)
			{
				tasks.Add(Task.Run(() =>
				{
					mres.Wait();
					_cache.InviteParticipant();
				}));
			}

			for (var sub = 0; sub < subAmount; sub++)
			{
				tasks.Add(Task.Run(() =>
				{
					mres.Wait();
					_cache.ExitParticipant();
				}));
			}

			mres.Set();

			await Task.WhenAll(tasks).ConfigureAwait(false);

			_cache.Participants
				.Should().Be(addAmount - subAmount);
		}

		[Fact]
		public void EnsureCachedItemsAfterReadingAreDeleted()
		{
			// 2 participants

			_cache.InviteParticipant();
			_cache.InviteParticipant();

			// we will add some items

			var current = _cache.Top;

			_cache.AppendItem(1);

			_cache.Last.Should().Be(0);
			_cache.Top.Should().Be(1);

			_cache.AppendItem(2);

			_cache.Last.Should().Be(0);
			_cache.Top.Should().Be(2);

			_cache.AppendItem(3);

			_cache.Last.Should().Be(0);
			_cache.Top.Should().Be(3);

			// now let's read the 3

			var _ = _cache[current];

			_cache.Last.Should().Be(0);
			_cache.Top.Should().Be(3);

			_ = _cache[current + 1];

			_cache.Last.Should().Be(0);
			_cache.Top.Should().Be(3);

			_ = _cache[current + 2];

			_cache.Last.Should().Be(0);
			_cache.Top.Should().Be(3);

			// so now the other participant just needs to read them
			// reading shouldn't cause any trouble

			_ = _cache[current];

			_cache.Last.Should().Be(1);
			_cache.Top.Should().Be(3);

			_ = _cache[current + 1];

			_cache.Last.Should().Be(2);
			_cache.Top.Should().Be(3);

			// but if we try to rea the others now that we already read them,
			// tht should cause problems since there are only suppose to be 2 readers

			Action shouldThrow1 = () => _ = _cache[current];
			Action shouldThrow2 = () => _ = _cache[current + 1];

			shouldThrow1.Should().Throw<Exception>();
			shouldThrow2.Should().Throw<Exception>();

			_cache.Last.Should().Be(2);
			_cache.Top.Should().Be(3);

			_ = _cache[current + 2];

			_cache.Last.Should().Be(3);
			_cache.Top.Should().Be(3);
		}

		[Fact]
		public void GetFactory_TriggersWhenDoesntExist_GetsWhenDoes()
		{
			_cache.InviteParticipant();
			_cache.InviteParticipant();

			bool didFactory = false;

			_cache.Get(0, () =>
			{
				didFactory = true;
				return 0;
			});

			didFactory.Should().BeTrue();
			didFactory = false;

			_cache.Get(0, () =>
			{
				didFactory = true;
				return 0;
			});

			didFactory.Should().BeFalse();

			Action throws = () =>
			{
				var i = _cache[0];
			};

			throws.Should().Throw<Exception>();
		}

		[Fact]
		public async Task MultithreadedTest()
		{
			const int max = 10_000;

			var mres = new ManualResetEventSlim();

			void TaskCode()
			{
				mres.Wait();

				var current = 0;

				for (var i = 0; i < max; i++)
				{
					_cache.Get(current, () => current)
						.Should().Be(current);
					current++;
				}
			}

			const int participants = 10;

			var tasks = new Task[participants];

			for (var i = 0; i < participants; i++)
			{
				_cache.InviteParticipant();
				tasks[i] = Task.Run(TaskCode);
			}

			mres.Set();

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}
	}
}
