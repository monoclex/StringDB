using JetBrains.Annotations;

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
	[PublicAPI]
	public sealed class ThreadLockDatabase<TKey, TValue> : BaseDatabase<TKey, TValue>
	{
		private sealed class ThreadLoackLazyLoader : ILazyLoader<TValue>
		{
			private readonly object _lock;
			private readonly ILazyLoader<TValue> _inner;

			public ThreadLoackLazyLoader([NotNull] object @lock, [NotNull] ILazyLoader<TValue> inner)
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

		private sealed class ThinDatabaseIEnumeratorWrapper : IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>>, IEnumerator<KeyValuePair<TKey, ILazyLoader<TValue>>>
		{
			private readonly object _lock;
			private readonly IEnumerator<KeyValuePair<TKey, ILazyLoader<TValue>>> _enumerator;
			private KeyValuePair<TKey, ILazyLoader<TValue>> _current;

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

					_current = new KeyValuePair<TKey, ILazyLoader<TValue>>
					(
						current.Key,
						new ThreadLoackLazyLoader(_lock, current.Value)
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

			public KeyValuePair<TKey, ILazyLoader<TValue>> Current => _current;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
				lock (_lock)
				{
					_enumerator.Dispose();
				}
			}

			public IEnumerator<KeyValuePair<TKey, ILazyLoader<TValue>>> GetEnumerator() => this;

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		private readonly IDatabase<TKey, TValue> _db;
		private readonly object _lock;

		/// <summary>
		/// Creates a new <see cref="ThreadLockDatabase{TKey,TValue}"/> around a database.
		/// </summary>
		/// <param name="database">The database to intelligently lock on.</param>
		public ThreadLockDatabase(IDatabase<TKey, TValue> database)
		{
			_lock = new object();
			_db = database;
		}

		/// <inheritdoc />
		public override void InsertRange(KeyValuePair<TKey, TValue>[] items)
		{
			lock (_lock)
			{
				_db.InsertRange(items);
			}
		}

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>> Evaluate()
			=> new ThinDatabaseIEnumeratorWrapper(_lock, _db);

		/// <inheritdoc />
		public override void Dispose()
		{
			lock (_lock)
			{
				_db.Dispose();
			}
		}
	}
}