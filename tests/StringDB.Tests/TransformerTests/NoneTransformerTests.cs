using FluentAssertions;

using StringDB.Transformers;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests for a <see cref="NoneTransformer{T}"/>
	/// </summary>
	public class NoneTransformerTests
	{
		[Fact]
		public void DoesLiterallyNothing()
		{
			var a = new NoneTransformer<int>();

			a.TransformPre(7)
				.Should().Be(7);

			a.TransformPost(7)
				.Should().Be(7);
		}
	}
}