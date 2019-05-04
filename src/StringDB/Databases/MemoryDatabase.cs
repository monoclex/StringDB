using JetBrains.Annotations;

using StringDB.LazyLoaders;

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
		private readonly List<KeyValuePair<TKey, TValue>> _data;

		/// <summary>
		/// Create a new <see cref="MemoryDatabase{TKey,TValue}"/>.
		/// </summary>
		/// <param name="data">The data to pre-fill it with.</param>
		public MemoryDatabase([CanBeNull] List<KeyValuePair<TKey, TValue>> data = null)
			: this(data, EqualityComparer<TKey>.Default)
		{
		}

		/// <summary>
		/// Create a new <see cref="MemoryDatabase{TKey,TValue}"/>.
		/// </summary>
		/// <param name="data">The data to pre-fill it with.</param>
		/// <param name="comparer">The equality comparer to use.</param>
		public MemoryDatabase([CanBeNull] List<KeyValuePair<TKey, TValue>> data, [NotNull] IEqualityComparer<TKey> comparer)
			: base(comparer)
			=> _data = data ?? new List<KeyValuePair<TKey, TValue>>();

		/// <inheritdoc />
		public override void InsertRange(params KeyValuePair<TKey, TValue>[] items)
			=> _data.AddRange(items);

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>> Evaluate()
			=> _data
			.Select
			(
				item => new ValueLoader<TValue>(item.Value)
					.ToKeyValuePair(item.Key)
			);

		/// <inheritdoc />
		public override void Dispose() => _data.Clear();
	}
}