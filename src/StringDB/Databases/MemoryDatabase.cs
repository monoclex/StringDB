using System.Collections.Generic;
using System.Linq;

namespace StringDB.Databases
{
	/// <summary>
	/// A database entire in memory.
	/// </summary>
	/// <typeparam name="TKey">The type of key of the database.</typeparam>
	/// <typeparam name="TValue">The type of value of the database.</typeparam>
	public sealed class MemoryDatabase<TKey, TValue> : BaseDatabase<TKey, TValue>
	{
		private sealed class LazyValueLoader : ILazyLoading<TValue>
		{
			private readonly TValue _value;

			public LazyValueLoader(TValue value) => _value = value;

			public TValue Load() => _value;
		}

		private readonly List<KeyValuePair<TKey, TValue>> _data;

		public MemoryDatabase(List<KeyValuePair<TKey, TValue>> data = null) => _data = data ?? new List<KeyValuePair<TKey, TValue>>();

		public override void InsertRange(KeyValuePair<TKey, TValue>[] items)
			=> _data.AddRange(items);

		protected override IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>> Evaluate()
			=> _data
			.Select
			(
				item => new KeyValuePair<TKey, ILazyLoading<TValue>>
				(
					key: item.Key,
					value: new LazyValueLoader(item.Value)
				)
			);

		public override void Dispose() => _data.Clear();
	}
}