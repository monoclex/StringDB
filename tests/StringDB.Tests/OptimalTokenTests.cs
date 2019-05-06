using FluentAssertions;

using StringDB.IO;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests for an <see cref="OptimalTokenSource"/>
	/// </summary>
	public class OptimalTokenTests
	{
		/// <summary>
		/// Test that we can set the token to an on and off state.
		/// </summary>
		[Fact]
		public void SettingTokenStateWorks()
		{
			IOptimalTokenSource optimalToken = new OptimalTokenSource();

			optimalToken.OptimalReadingTime.Should().Be(false);

			SetRead(true);
			SetRead(false);

			void SetRead(bool value)
			{
				optimalToken.SetOptimalReadingTime(value);
				optimalToken.OptimalReadingTime.Should().Be(value);
			}
		}
	}
}