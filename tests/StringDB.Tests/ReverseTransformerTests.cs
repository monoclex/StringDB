using FluentAssertions;

using StringDB.Transformers;

using System;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests for a <see cref="ReverseTransformer{TPost, TPre}"/>.
	/// </summary>
	public class ReverseTransformerTests
	{
		// class setup
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
			public B TransformPre(A pre) => new B { OtherValue = pre.Value };

			public A TransformPost(B post) => new A { Value = post.OtherValue };
		}

		/// <summary>
		/// Tests that the reverse transformer performs the exact same transformations
		/// as a regular transformer, but reverses the position of stuff.
		/// </summary>
		[Fact]
		public void Reverses()
		{
			const string because = "The transformation should call the appropriate method in the underlying transformer";

			var reverse = new ReverseTransformer<B, A>(new SimpleTransformer());

			// in SimpleTransformer this is TransformPost
			reverse.TransformPre(new B { OtherValue = 3 })
				.Value
				.Should()
				.Be(3, because);

			// in SimpleTransformer this is TransformPre
			reverse.TransformPost(new A { Value = 4 })
				.OtherValue
				.Should()
				.Be(4, because);
		}

		/// <summary>
		/// Test that the Reverse call reverses a transformer.
		/// </summary>
		[Fact]
		public void ExtensionWorks()
		{
			new SimpleTransformer()
				.Reverse()
				.Should()
				.BeOfType<ReverseTransformer<B, A>>("A reversed SimpleTransformer");
		}

		/// <summary>
		/// When reversing a transformer twice, the original transformer should be returned.
		/// </summary>
		[Fact]
		public void TwoReversesShouldReturnOriginalTransformer()
		{
			new SimpleTransformer()
				.Reverse()
				.Reverse()
				.Should()
				.BeOfType<SimpleTransformer>("Reversing a transformer twice should result in the original transformer.");
		}
	}
}