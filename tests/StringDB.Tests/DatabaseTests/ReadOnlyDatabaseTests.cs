using FluentAssertions;

using Moq;

using StringDB.Fluency;

using System;
using System.Collections.Generic;

using Xunit;

namespace StringDB.Tests.DatabaseTests
{
	public class ReadOnlyDatabaseTests
	{
		private readonly Mock<IDatabase<int, int>> _mock;
		private readonly IDatabase<int, int> _rdb;
		private readonly IDatabaseLayer<int, int> _dbLayer;

		public ReadOnlyDatabaseTests()
		{
			_mock = new Mock<IDatabase<int, int>>();

			_rdb = _mock.Object.AsReadOnly();
			_dbLayer = (IDatabaseLayer<int, int>)_rdb;
		}

		[Fact]
		public void InnerDatabase_IsDatabase()
		{
			_dbLayer.InnerDatabase.As<object>()
				.Should().Be(_mock.Object);
		}

		[Fact]
		public void Get_ShouldCall()
		{
			_rdb.Get(0);

			_mock.Verify(x => x.Get(0));
		}

		[Fact]
		public void TryGet_ShouldCall()
		{
			_rdb.TryGet(0, out _);

			_mock.Verify(x => x.TryGet(0, out It.Ref<int>.IsAny));
		}

		[Fact]
		public void GetAll_ShouldCall()
		{
			_rdb.GetAll(0);

			_mock.Verify(x => x.GetAll(0));
		}

		[Fact]
		public void Insert_ShouldThrow()
		{
			Action throws = () => _rdb.Insert(0, 0);

			throws.Should().Throw<Exception>();
		}

		[Fact]
		public void InsertRange_ShouldThrow()
		{
			Action throws = () => _rdb.InsertRange(new KeyValuePair<int, int>(0, 0));

			throws.Should().Throw<Exception>();
		}
	}
}