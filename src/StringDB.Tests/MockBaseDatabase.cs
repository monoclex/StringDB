using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringDB.Tests
{
	public class MockBaseDatabase : BaseDatabase<string, int>, IMockDatabase
	{
		public HashSet<KeyValuePair<string, LazyInt>> Data { get; } = new MockDatabase().Data;

		public KeyValuePair<string, int>[] Inserted { get; set; }

		public override void InsertRange(KeyValuePair<string, int>[] items) => Inserted = items;

		protected override IEnumerable<KeyValuePair<string, ILazyLoading<int>>> Evaluate()
			=> Data.Select(x => new KeyValuePair<string, ILazyLoading<int>>(x.Key, x.Value));
	}
}
