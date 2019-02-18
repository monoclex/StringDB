using System.Collections;
using System.Collections.Generic;

namespace StringDB
{
	public abstract class BaseDatabase<TKey, TValue> : IDatabase<TKey, TValue>
	{
		private readonly EqualityComparer<TKey> _keyComparer;

		public BaseDatabase()
			=> _keyComparer = EqualityComparer<TKey>.Default;

		public abstract void InsertRange(KeyValuePair<TKey, TValue>[] items);

		protected abstract IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>> Evaluate();

		public TValue Get(TKey key)
		{
			if (!TryGet(key, out var value))
			{
				throw new KeyNotFoundException($"Unable to find {key} in the database.");
			}

			return value;
		}

		public bool TryGet(TKey key, out TValue value)
		{
			foreach (var result in GetAll(key))
			{
				value = result;
				return true;
			}

			value = default;
			return false;
		}

		public void Insert(TKey key, TValue value)
			=> InsertRange(new KeyValuePair<TKey, TValue>[] { new KeyValuePair<TKey, TValue>(key, value) });

		public IEnumerable<TValue> GetAll(TKey key)
		{
			foreach (var item in Evaluate())
			{
				if (_keyComparer.Equals(key, item.Key))
				{
					yield return item.Value.Load();
				}
			}
		}

		public IEnumerator<KeyValuePair<TKey, ILazyLoading<TValue>>> GetEnumerator() => Evaluate().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}