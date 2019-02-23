using StringDB.Databases;

using System;
using System.Collections.Generic;
using System.Linq;

namespace StringDB.Tests
{
	public class MockBaseDatabase : BaseDatabase<string, int>, IMockDatabase<string, int>
	{
		public List<KeyValuePair<string, LazyItem<int>>> Data { get; } = new MockDatabase().Data;

		public KeyValuePair<string, int>[] Inserted { get; set; }

		public override void InsertRange(KeyValuePair<string, int>[] items) => Inserted = items;

		protected override IEnumerable<KeyValuePair<string, ILazyLoader<int>>> Evaluate()
			=> Data.Select(x => new KeyValuePair<string, ILazyLoader<int>>(x.Key, x.Value));

		public IEnumerable<KeyValuePair<string, ILazyLoader<int>>> Enumerator() => Evaluate();

		public override void Dispose() => throw new NotImplementedException();
	}
}