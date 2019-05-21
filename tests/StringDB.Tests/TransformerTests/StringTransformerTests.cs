using FluentAssertions;

using StringDB.Transformers;

using System.Text;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// <see cref="StringTransformer"/> tests.
	/// </summary>
	public class StringTransformerTests
	{
		private readonly StringTransformer _transformer;
		private readonly string _testingString;
		private readonly byte[] _testingBytes;

		public StringTransformerTests()
		{
			_transformer = new StringTransformer();

			_testingString = "test";
			_testingBytes = Encoding.UTF8.GetBytes(_testingString);
		}

		/// <summary>
		/// The TransformPost method should transform the string into the correct bytes.
		/// </summary>
		[Fact]
		public void TransformPostTest()
			=> _transformer.TransformPost(_testingString)
			.Should()
			.BeEquivalentTo(_testingBytes);

		/// <summary>
		/// The TransformPre method should transform the bytes into the correct string.
		/// </summary>
		[Fact]
		public void TransformPreTest()
			=> _transformer.TransformPre(_testingBytes)
			.Should()
			.BeEquivalentTo(_testingString);
	}
}