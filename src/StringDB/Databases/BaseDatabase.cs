using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringDB.Databases
{
	/// <inheritdoc />
	/// <summary>
	/// An implementor of IDatabase that requires the inheriting class
	/// to only implement two functions.
	/// </summary>
	/// <typeparam name="TKey">The type of key of the database.</typeparam>
	/// <typeparam name="TValue">The type of value of the database.</typeparam>
	public abstract class BaseDatabase<TKey, TValue> : IDatabase<TKey, TValue>
	{
		private readonly EqualityComparer<TKey> _keyComparer;

		protected BaseDatabase()
			=> _keyComparer = EqualityComparer<TKey>.Default;

		/// <inheritdoc />
		public abstract void InsertRange(KeyValuePair<TKey, TValue>[] items);

		/// <summary>
		/// Enumerates over all the items in the database.
		/// </summary>
		/// <returns>An IEnumerable of KeyValuePairs of keys and their lazy-loading values.</returns>
		protected abstract IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>> Evaluate();

		/// <inheritdoc />
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">When the key is unable to be found.</exception>
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

		/// <summary>
		/// Cleans up any resources the database is using.
		/// </summary>
		public abstract void Dispose();
	}
}