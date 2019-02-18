using System.Collections.Generic;

namespace StringDB
{
	public sealed class MemoryDatabase<TKey, TValue> : BaseDatabase<TKey, TValue>
	{
		private sealed class LazyValueLoader : ILazyLoading<TValue>
		{
			private readonly TValue _value;

			public LazyValueLoader(TValue value) => _value = value;

			public TValue Load() => _value;
		}

		private readonly HashSet<KeyValuePair<TKey, TValue>> _data;

		public MemoryDatabase(HashSet<KeyValuePair<TKey, TValue>> data = null) => _data = data ?? new HashSet<KeyValuePair<TKey, TValue>>();

		public override void InsertRange(KeyValuePair<TKey, TValue>[] items)
		{
			foreach (var item in items)
			{
				_data.Add(item);
			}
		}

		protected override IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>> Evaluate()
		{
			foreach (var item in _data)
			{
				yield return new KeyValuePair<TKey, ILazyLoading<TValue>>
				(
					key: item.Key,
					value: new LazyValueLoader(item.Value)
				);
			}
		}
	}
}