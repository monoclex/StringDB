using FluentAssertions;
using StringDB.Querying;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace StringDB.Tests.QueryingTests
{
	public class DatabaseEnumerableTests
	{
		[Fact]
		public void ModifyValue()
		{
			var current = 0;
			foreach(var i in DatabaseEnumerable.ModifyValue(new KeyValuePair<int, int>[]
			{
				KeyValuePair.Create(0, 0),
				KeyValuePair.Create(1, 1),
				KeyValuePair.Create(2, 2),
				KeyValuePair.Create(3, 3),
				KeyValuePair.Create(4, 4),
			}, @in => @in * 5))
			{
				i.Key.Should().Be(current);
				i.Value.Should().Be(current * 5);

				current++;
			}
		}

		[Fact]
		public void LockingEnumerable()
		{
			var integers = new int[] { 1, 2, 3 };

			// i really don't know how to test that this works
			// maybe if i were to mock the lock that'd probably work
			var @lock = new SemaphoreSlim(1);

			foreach(var i in integers.EnumerateWithLocking(@lock))
			{
				// the lock shouldn't be held at this point at least

				@lock.Wait();
				@lock.Release();
			}
		}

		// the make train enumerable will be tested by the query manager part of the code
	}
}
