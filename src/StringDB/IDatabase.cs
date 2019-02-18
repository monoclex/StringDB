using System.Collections.Generic;

namespace StringDB
{
	public interface IDatabase<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>>
	{
		TValue Get(TKey key);

		bool TryGet(TKey key, out TValue value);

		IEnumerable<TValue> GetAll(TKey key);

		void Insert(TKey key, TValue value);

		void InsertRange(KeyValuePair<TKey, TValue>[] items);
	}
}