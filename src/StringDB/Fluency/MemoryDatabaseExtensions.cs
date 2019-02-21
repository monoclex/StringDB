using StringDB.Databases;

using System.Collections.Generic;

namespace StringDB.Fluency
{
	public static class MemoryDatabaseExtensions
	{
		public static IDatabase<TKey, TValue> UseMemoryDatabase<TKey, TValue>
		(
			this DatabaseBuilder builder
		)
			=> builder.UseMemoryDatabase<TKey, TValue>(null);

		public static IDatabase<TKey, TValue> UseMemoryDatabase<TKey, TValue>
		(
			this DatabaseBuilder builder,
			List<KeyValuePair<TKey, TValue>> data
		)
			=> new MemoryDatabase<TKey, TValue>(data);
	}
}