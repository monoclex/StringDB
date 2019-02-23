using System.Collections.Generic;

namespace StringDB.Tests
{
	public interface IMockDatabase<TKey, TValue>
	{
		List<KeyValuePair<TKey, LazyItem<TValue>>> Data { get; }
	}
}