using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests
{
	public class ReverseTransformerTests
	{
		public class A
		{
			public int Value { get; set; }
		}

		public class B
		{
			public int OtherValue { get; set; }
		}

		public class SimpleTransformer : ITransformer<A, B>
		{
			public B Transform(A pre) => new B { OtherValue = pre.Value };
			public A Transform(B post) => new A { Value = post.OtherValue };
		}

		[Fact]
		public void Reverses()
		{
			var reverse = new ReverseTransformer<B, A>(new SimpleTransformer());

			reverse.Transform(new B { OtherValue = 3 })
				.Value
				.Should()
				.Be(3);

			reverse.Transform(new A { Value = 4 })
				.OtherValue
				.Should()
				.Be(4);
		}
	}
}
