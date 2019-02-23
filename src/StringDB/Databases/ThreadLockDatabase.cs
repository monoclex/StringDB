using System.Collections;
using System.Collections.Generic;

namespace StringDB.Databases
{
	/// <inheritdoc />
	/// <summary>
	/// Intelligently uses locks to force and ensure that
	/// two threads never access the database at once.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	public sealed class ThreadLockDatabase<TKey, TValue> : BaseDatabase<TKey, TValue>
	{
		private sealed class ThreadLazyLoader : ILazyLoading<TValue>
		{
			private readonly object _lock;
			private readonly ILazyLoading<TValue> _inner;

			public ThreadLazyLoader(object @lock, ILazyLoading<TValue> inner)
			{
				_lock = @lock;
				_inner = inner;
			}

			public TValue Load()
			{
				lock (_lock)
				{
					return _inner.Load();
				}
			}
		}

		private sealed class ThinDatabaseIEnumeratorWrapper : IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>>, IEnumerator<KeyValuePair<TKey, ILazyLoading<TValue>>>
		{
			private readonly object _lock;
			private readonly IEnumerator<KeyValuePair<TKey, ILazyLoading<TValue>>> _enumerator;
			private KeyValuePair<TKey, ILazyLoading<TValue>> _current;

			public ThinDatabaseIEnumeratorWrapper(object @lock, IDatabase<TKey, TValue> database)
			{
				_lock = @lock;

				lock (_lock)
				{
					_enumerator = database.GetEnumerator();
				}
			}

			public bool MoveNext()
			{
				lock (_lock)
				{
					var result = _enumerator.MoveNext();

					if (!result) return false;

					var current = _enumerator.Current;

					_current = new KeyValuePair<TKey, ILazyLoading<TValue>>
					(
						current.Key,
						new ThreadLazyLoader(_lock, current.Value)
					);

					return true;
				}
			}

			public void Reset()
			{
				lock (_lock)
				{
					_enumerator.Reset();
				}
			}

			public KeyValuePair<TKey, ILazyLoading<TValue>> Current => _current;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
				lock (_lock)
				{
					_enumerator.Dispose();
				}
			}

			public IEnumerator<KeyValuePair<TKey, ILazyLoading<TValue>>> GetEnumerator() => this;

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		private readonly IDatabase<TKey, TValue> _db;
		private readonly object _lock;

		public ThreadLockDatabase(IDatabase<TKey, TValue> database)
		{
			_lock = new object();
			_db = database;
		}

		public override void InsertRange(KeyValuePair<TKey, TValue>[] items)
		{
			lock (_lock)
			{
				_db.InsertRange(items);
			}
		}

		protected override IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>> Evaluate()
			=> new ThinDatabaseIEnumeratorWrapper(_lock, _db);

		public override void Dispose()
		{
			lock (_lock)
			{
				_db.Dispose();
			}
		}
	}
}