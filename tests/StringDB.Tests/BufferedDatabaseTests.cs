using FluentAssertions;
using StringDB.Databases;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringDB.Tests
{
	public class BufferedDatabaseTests : IDisposable
	{
		public class MockDatabase : BaseDatabase<int, int>
		{
			public bool Disposed { get; set; }
			public int Inserts { get; set; }
			public int Evaluations { get; set; }

			public override void Dispose() => Disposed = true;

			public override void InsertRange(KeyValuePair<int, int>[] items) => Inserts++;

			protected override IEnumerable<KeyValuePair<int, ILazyLoader<int>>> Evaluate()
			{
				Evaluations++;
				yield break;
			}
		}

		private readonly MockDatabase _mockDb;
		private readonly BufferedDatabase<int, int> _db;
		public const int BufferSize = BufferedDatabase<int, int>.MinimumBufferSize;

		public BufferedDatabaseTests()
		{
			_mockDb = new MockDatabase();
			_db = new BufferedDatabase<int, int>(_mockDb, BufferSize);
		}

		[Fact]
		public void SingleInsertCallsNoInserts()
		{
			_db.Insert(0, 0);

			_mockDb.Inserts
				.Should().Be(0);
		}

		[Fact]
		public void FillBuffer()
		{
			for (var i = 0; i < BufferSize; i++)
			{
				_db.Insert(0, 0);
			}

			_mockDb.Inserts
				.Should().Be(0);

			_db.Insert(0, 0);

			_mockDb.Inserts
				.Should().Be(1);
		}

		public void Dispose()
		{
			_mockDb.Disposed
				.Should().BeFalse();

			_db.Dispose();

			_mockDb.Disposed
				.Should().BeTrue();

			// be at least one
			_mockDb.Inserts
				.Should().BeGreaterOrEqualTo(1);
		}
	}
}
