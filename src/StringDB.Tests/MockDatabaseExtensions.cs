using FluentAssertions;

using System.Linq;

namespace StringDB.Tests
{
	public static class MockDatabaseExtensions
	{
		public static void EnsureNoValuesLoaded<TKey, TValue>(this IMockDatabase<TKey, TValue> mdb)
			=> mdb.Data.Where(x => x.Value.Loaded)
			.Should()
			.BeEmpty("No lazy values should be loaded with this operation");

		public static void EnsureAllValuesLoaded<TKey, TValue>(this IMockDatabase<TKey, TValue> mdb)
			=> mdb.Data.Where(kvp => !kvp.Value.Loaded)
			.Should()
			.BeEmpty("All values should be loaded");

		public static void EnsureNoValuesLoadedBeyond<TKey, TValue>(this IMockDatabase<TKey, TValue> mdb, int position)
			=> mdb.Data.Where((kvp, index) => index <= position ? !kvp.Value.Loaded : kvp.Value.Loaded)
			.Should()
			.BeEmpty("No values beyond the current enumeration point should be loaded");
	}
}