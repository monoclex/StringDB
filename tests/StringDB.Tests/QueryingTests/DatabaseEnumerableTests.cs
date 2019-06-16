using FluentAssertions;
using StringDB.Querying;
using System;
using System.Collections.Generic;
using System.Text;
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
	}
}
