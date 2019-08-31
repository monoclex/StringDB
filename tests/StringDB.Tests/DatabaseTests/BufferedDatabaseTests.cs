using FluentAssertions;

using StringDB.Databases;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace StringDB.Tests
{
	public class BufferedDatabaseTests
	{
		public class MockDatabase : BaseDatabase<int, int>
		{
			public bool Disposed { get; set; }
			public int Inserts { get; set; }
			public int Evaluations { get; set; }

			public List<int> InsertedData { get; set; } = new List<int>();

			public override void Dispose() => Disposed = true;

			public override void InsertRange(params KeyValuePair<int, int>[] items)
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
			_db = new BufferedDatabase<int, int>(_mockDb, BufferSize, false);
		}

		[Fact]
		public void When_DbIsDisposed_BufferedItems_GetWritten()
		{
			for (var i = 0; i < BufferSize - 1; i++)
			{
				_db.Insert(i, i);
			}

			_db.Dispose();

			_mockDb.InsertedData.Count().Should().Be(BufferSize - 1);
		}

		[Fact]
		public void EvaluationIncludesBufferedItems()
		{
			_db.Insert(0, 1337);
			_mockDb.Inserts
				.Should().Be(0);

			// assures us that the buffered database evaluation
			// includes buffered entries
			_db.Count()
				.Should().Be(1);

			// the buffered db should also evaluate the inner database
			_mockDb.Evaluations
				.Should().Be(1);
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
				.Should().Be(1);

			_db.InsertRange(new[] { KeyValuePair.Create(BufferSize, 0) });

			_mockDb.Inserts
				.Should().Be(1);

			_mockDb.InsertedData
				.Should()
				.BeEquivalentTo(Enumerable.Range(0, BufferSize));
		}

		/*
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
		*/

		[Fact]
		public void LessThanMinimumBuffer_Throws()
		{
			// this testing class is BAD so i'm just gonna insert one to get the Dispose part over with
			_mockDb.Insert(0, 0);

			Throws(() => new BufferedDatabase<int, int>(_mockDb, BufferedDatabase<int, int>.MinimumBufferSize - 1));
		}

		private void Throws(Action throws)
			=> throws.Should().Throw<Exception>();
	}
}