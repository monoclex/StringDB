using FluentAssertions;

using StringDB.Transformers;

using System.Text;

using Xunit;

namespace StringDB.Tests
{
	public class StringTransformerTests
	{
		[Fact]
		public void StringIntoByte()
		{
			var t = new StringTransformer();

			t.TransformPost("test")
				.Should()
				.BeEquivalentTo(Encoding.UTF8.GetBytes("test"));
		}

		[Fact]
		public void ByteIntoString()
		{
			var t = new StringTransformer();

			var bytes = Encoding.UTF8.GetBytes("test");

			t.TransformPre(bytes)
				.Should()
				.BeEquivalentTo("test");
		}
	}
}