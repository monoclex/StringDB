using FluentAssertions;

using StringDB.Databases;
using StringDB.IO;
using StringDB.LazyLoaders;

using System;
using System.Collections.Generic;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests to verify that the optimal enumeration method is working.
	/// </summary>
	public class OptimalEnumerationTests
	{
		public class MockDb : BaseDatabase<int, int>
		{
			private readonly int _value;

			public MockDb(int value)
			{
				_value = value;
			}

			public OptimalToken OptimalToken { get; } = new OptimalToken();
			public bool EvaluateNext { get; set; } = true;

			public override void Dispose()
			{
			}

			public override void InsertRange(params KeyValuePair<int, int>[] items) => throw new NotImplementedException();

			protected override IEnumerable<KeyValuePair<int, ILazyLoader<int>>> Evaluate()
			{
				for (var i = 1; i <= _value; i++)
				{
					// we're evaluating
					EvaluateNext = true;
					OptimalToken.SetOptimalReadingTime(false);

					// we've hit 10, claim this to be the optimal time to stop reading
					if (i % 10 == 0)
					{
						// we set this to false, we shouldn't be evaluating any more
						EvaluateNext = false;
						OptimalToken.SetOptimalReadingTime(true);
					}

					// return the kvp
					yield return new ValueLoader<int>(i).ToKeyValuePair(i);
				}
			}
		}

		/// <summary>
		/// Every 10, it should enumerate optimally
		/// </summary>
		[Fact]
		public void OptimallyEnumeratesEveryTen()
		{
			var db = new MockDb(100);

			foreach (var (key, value) in db.EnumerateOptimally(db.OptimalToken))
			{
				db.EvaluateNext.Should().BeFalse();
			}
		}

		/// <summary>
		/// We never hit the optimal condition, so we should enumerate under an evaluated condition.
		/// </summary>
		[Fact]
		public void IfNotOptimalEnumeratesAnyway()
		{
			var db = new MockDb(9);

			foreach (var (key, value) in db.EnumerateOptimally(db.OptimalToken))
			{
				db.EvaluateNext.Should().BeTrue();
			}
		}
	}
}