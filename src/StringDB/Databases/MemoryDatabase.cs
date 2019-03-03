using JetBrains.Annotations;

using System.Collections.Generic;
using System.Linq;

namespace StringDB.Databases
{
	/// <inheritdoc />
	/// <summary>
	/// A database entire in memory.
	/// </summary>
	/// <typeparam name="TKey">The type of key of the database.</typeparam>
	/// <typeparam name="TValue">The type of value of the database.</typeparam>
	[PublicAPI]
	public sealed class MemoryDatabase<TKey, TValue> : BaseDatabase<TKey, TValue>
	{
		private sealed class MemoryLazyLoader : ILazyLoader<TValue>
		{
			private readonly TValue _value;

			public MemoryLazyLoader([NotNull] TValue value) => _value = value;

			public TValue Load() => _value;
		}

		private readonly List<KeyValuePair<TKey, TValue>> _data;

		/// <summary>
		/// Create a new <see cref="MemoryDatabase{TKey,TValue}"/>.
		/// </summary>
		/// <param name="data">The data to pre-fill it with.</param>
		public MemoryDatabase([CanBeNull] List<KeyValuePair<TKey, TValue>> data = null)
			=> _data = data ?? new List<KeyValuePair<TKey, TValue>>();

		/// <inheritdoc />
		public override void InsertRange(KeyValuePair<TKey, TValue>[] items)
			=> _data.AddRange(items);

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>> Evaluate()
			=> _data
			.Select
			(
				item => new KeyValuePair<TKey, ILazyLoader<TValue>>
				(
					key: item.Key,
					value: new MemoryLazyLoader(item.Value)
				)
			);

		/// <inheritdoc />
		public override void Dispose() => _data.Clear();
	}
}