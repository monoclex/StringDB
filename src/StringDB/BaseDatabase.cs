using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringDB
{
	public abstract class BaseDatabase<TKey, TValue> : IDatabase<TKey, TValue>
	{
		private readonly EqualityComparer<TKey> _keyComparer;

		protected BaseDatabase()
			=> _keyComparer = EqualityComparer<TKey>.Default;

		public abstract void InsertRange(KeyValuePair<TKey, TValue>[] items);

		protected abstract IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>> Evaluate();

		public TValue Get(TKey key)
			=> TryGet(key, out var value)
			? value
			: throw new KeyNotFoundException($"Unable to find {key} in the database.");

		public bool TryGet(TKey key, out TValue value)
		{
			foreach (var result in GetAll(key))
			{
				value = result.Load();
				return true;
			}

			value = default;
			return false;
		}

		public void Insert(TKey key, TValue value)
			=> InsertRange(new KeyValuePair<TKey, TValue>[] { new KeyValuePair<TKey, TValue>(key, value) });

		public IEnumerable<ILazyLoading<TValue>> GetAll(TKey key)
			=> Evaluate()
			.Where(item => _keyComparer.Equals(key, item.Key))
			.Select(item => item.Value);

		public IEnumerator<KeyValuePair<TKey, ILazyLoading<TValue>>> GetEnumerator() => Evaluate().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}