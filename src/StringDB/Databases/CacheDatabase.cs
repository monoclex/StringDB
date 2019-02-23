using System;
using System.Collections.Generic;

namespace StringDB.Databases
{
	/// <inheritdoc />
	/// <summary>
	/// Non-thread-safely caches results and loaded values from a database.
	/// This could be used in scenarios where you're repeatedly iterating
	/// over an IODatabase.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public sealed class CacheDatabase<TKey, TValue> : BaseDatabase<TKey, TValue>
	{
		private sealed class CacheLazyLoader : ILazyLoading<TValue>, IDisposable
		{
			private readonly ILazyLoading<TValue> _inner;

			private bool _loaded;
			private TValue _value;

			public CacheLazyLoader(ILazyLoading<TValue> inner)
				=> _inner = inner;

			public TValue Load()
			{
				if (!_loaded)
				{
					_value = _inner.Load();
					_loaded = true;
				}

				return _value;
			}

			public void Dispose()
			{
				if (_value is IDisposable disposable)
				{
					disposable.Dispose();
				}

				_value = default;
			}
		}

		private readonly List<KeyValuePair<TKey, CacheLazyLoader>> _cache;
		private readonly IDatabase<TKey, TValue> _database;

		/// <summary>
		/// Create a new <see cref="CacheDatabase{TKey,TValue}"/>.
		/// </summary>
		/// <param name="database">The database to cache.</param>
		public CacheDatabase(IDatabase<TKey, TValue> database)
		{
			_cache = new List<KeyValuePair<TKey, CacheLazyLoader>>();
			_database = database;
		}

		/// <inheritdoc />
		public override void InsertRange(KeyValuePair<TKey, TValue>[] items)

			// we can't add it to our cache otherwise it might change the order of things
			=> _database.InsertRange(items);

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>> Evaluate()
		{
			var counter = 0;

			foreach (var item in _database)
			{
				if (_cache.Count <= counter)
				{
					_cache.Add
					(
						new KeyValuePair<TKey, CacheLazyLoader>
						(
							item.Key,
							new CacheLazyLoader(item.Value)
						)
					);
				}

				var current = _cache[counter];
				yield return new KeyValuePair<TKey, ILazyLoading<TValue>>(current.Key, current.Value);

				counter++;
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			foreach (var item in _cache)
			{
				if (item.Key is IDisposable disposable)
				{
					disposable.Dispose();
				}

				item.Value.Dispose();
			}

			_cache.Clear();
			_database.Dispose();
		}
	}
}