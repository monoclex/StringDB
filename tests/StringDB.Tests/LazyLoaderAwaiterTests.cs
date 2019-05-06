using FluentAssertions;

using System.Threading.Tasks;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests for a <see cref="LazyLoaderAwaiter{T}"/>.
	/// </summary>
	public class LazyLoaderAwaiterTests
	{
		public const int Value = 7;
		private readonly LazyLoaderMock _mock;
		private readonly LazyLoaderAwaiter<int> _awaiter;

		public class LazyLoaderMock : ILazyLoader<int>
		{
			private readonly int _value;
			public int Loads { get; set; }

			public LazyLoaderMock(int value = Value) => _value = value;

			public int Load()
			{
				Loads++;
				return _value;
			}
		}

		public LazyLoaderAwaiterTests()
		{
			_mock = new LazyLoaderMock();
			_awaiter = new LazyLoaderAwaiter<int>
			{
				LazyLoader = _mock
			};
		}

		/// <summary>
		/// Tests that the await keyword has the same functionality as Load
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task AwaitLazyLoader()
		{
			var value = await _mock;

			value.Should().Be(Value);
			_mock.Loads.Should().Be(1);
		}

		[Fact]
		public void LazyLoaderAwaiter_LoadsALazyLoader()
		{
			_mock.Loads.Should().Be(0);
			_awaiter.IsCompleted.Should().BeFalse();

			_awaiter.GetResult().Should().Be(Value);

			_mock.Loads.Should().Be(1);
			_awaiter.IsCompleted.Should().BeTrue();
		}
	}
}