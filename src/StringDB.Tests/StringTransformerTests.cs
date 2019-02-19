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

			t.Transform("test")
				.Should()
				.BeEquivalentTo(Encoding.UTF8.GetBytes("test"));
		}

		[Fact]
		public void ByteIntoString()
		{
			var t = new StringTransformer();

			var bytes = Encoding.UTF8.GetBytes("test");

			t.Transform(bytes)
				.Should()
				.BeEquivalentTo("test");
		}
	}
}