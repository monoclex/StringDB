using FluentAssertions;

using StringDB.Databases;

using System;
using System.Collections.Generic;
using System.Linq;

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

			public List<int> InsertedData { get; set; } = new List<int>();

			public override void Dispose() => Disposed = true;

			public override void InsertRange(KeyValuePair<int, int>[] items)
			{
				Inserts++;
				InsertedData.InsertRange(InsertedData.Count, items.Select(x => x.Key).ToArray());
			}

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
		public void EvaluationIsSameAsMock()
		{
			_db.GetEnumerator()
				.Should()
				.BeOfType(_mockDb.GetEnumerator().GetType());

			// pass Dispose test
			_db.Insert(0, 0);
		}

		[Fact]
		public void SingleInsertCallsNoInserts()
		{
			_db.Insert(0, 0);

			_mockDb.Inserts
				.Should().Be(0);
		}

		[Fact]
		public void FillBufferWithInsert()
		{
			for (var i = 0; i < BufferSize; i++)
			{
				_db.Insert(i, 0);
			}

			_mockDb.Inserts
				.Should().Be(0);

			_db.Insert(BufferSize, 0);

			_mockDb.Inserts
				.Should().Be(1);

			_mockDb.InsertedData
				.Should()
				.BeEquivalentTo(Enumerable.Range(0, BufferSize));
		}

		[Fact]
		public void SingleInsertRangeCallsNoInserts()
		{
			_db.InsertRange(new[] { KeyValuePair.Create(0, 0) });

			_mockDb.Inserts
				.Should().Be(0);
		}

		[Fact]
		public void FillBufferWithInsertRange()
		{
			for (var i = 0; i < BufferSize; i++)
			{
				_db.InsertRange(new[] { KeyValuePair.Create(i, 0) });
			}

			_mockDb.Inserts
				.Should().Be(0);

			_db.InsertRange(new[] { KeyValuePair.Create(BufferSize, 0) });

			_mockDb.Inserts
				.Should().Be(1);

			_mockDb.InsertedData
				.Should()
				.BeEquivalentTo(Enumerable.Range(0, BufferSize));
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