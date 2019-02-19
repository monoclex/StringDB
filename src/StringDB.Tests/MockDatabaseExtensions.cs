using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringDB.Tests
{
	public static class MockDatabaseExtensions
	{
		public static void EnsureNoValuesLoaded(this IMockDatabase mdb)
			=> mdb.Data.Where(x => x.Value.Loaded)
			.Should()
			.BeEmpty("No lazy values should be loaded with this operation");

		public static void EnsureAllValuesLoaded(this IMockDatabase mdb)
			=> mdb.Data.Where(kvp => !kvp.Value.Loaded)
			.Should()
			.BeEmpty("All values should be loaded");

		public static void EnsureNoValuesLoadedBeyond(this IMockDatabase mdb, int position)
			=> mdb.Data.Where((kvp, index) => index <= position ? !kvp.Value.Loaded : kvp.Value.Loaded)
			.Should()
			.BeEmpty("No values beyond the current enumeration point should be loaded");
	}
}
