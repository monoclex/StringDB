using FluentAssertions;

using Moq;

using StringDB.Fluency;

using System;
using System.Collections.Generic;

using Xunit;

namespace StringDB.Tests.DatabaseTests
{
	public class WriteOnlyDatabaseTests
	{
		private readonly Mock<IDatabase<int, int>> _mock;
		private readonly IDatabase<int, int> _wdb;
		private readonly IDatabaseLayer<int, int> _dbLayer;

		public WriteOnlyDatabaseTests()
		{
			_mock = new Mock<IDatabase<int, int>>();

			_wdb = _mock.Object.AsWriteOnly();
			_dbLayer = (IDatabaseLayer<int, int>)_wdb;
		}

		[Fact]
		public void InnerDatabase_IsDatabase()
		{
			_dbLayer.InnerDatabase.As<object>()
				.Should().Be(_mock.Object);
		}

		[Fact]
		public void Get_ShouldThrow()
			=> Throws(() => _wdb.Get(0));

		[Fact]
		public void TryGet_ShouldThrow()
			=> Throws(() => _wdb.TryGet(0, out _));

		[Fact]
		public void GetAll_ShouldThrow()
			=> Throws(() => _wdb.GetAll(0));

		[Fact]
		public void Insert_ShouldCall()
		{
			_wdb.Insert(0, 0);

			_mock.Verify(x => x.Insert(0, 0));
		}

		[Fact]
		public void InsertRange_ShouldCall()
		{
			_wdb.InsertRange(new KeyValuePair<int, int>(0, 0));

			_mock.Verify(x => x.InsertRange(It.IsAny<KeyValuePair<int, int>[]>()));
		}

		private void Throws(Action action)
			=> action.Should().Throw<Exception>();
	}
}