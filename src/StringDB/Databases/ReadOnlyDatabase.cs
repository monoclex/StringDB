using JetBrains.Annotations;

using System;
using System.Collections;
using System.Collections.Generic;

namespace StringDB.Databases
{
	/// <summary>
	/// A database which can only be read from.
	/// </summary>
	[PublicAPI]
	public class ReadOnlyDatabase<TKey, TValue>
		: IDatabase<TKey, TValue>, IDatabaseLayer<TKey, TValue>
	{
		private readonly bool _disposeDatabase;

		/// <summary>
		/// Creates a new <see cref="ReadOnlyDatabase{TKey, TValue}"/>.
		/// </summary>
		/// <param name="database">The database to make read only.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		public ReadOnlyDatabase([NotNull] IDatabase<TKey, TValue> database, bool disposeDatabase = true)
		{
			_disposeDatabase = disposeDatabase;
			InnerDatabase = database;
		}

		/// <inheritdoc/>
		[NotNull] public IDatabase<TKey, TValue> InnerDatabase { get; }

		/// <inheritdoc/>
		public void Dispose()
		{
			if (_disposeDatabase)
			{
				InnerDatabase.Dispose();
			}
		}

		private const string Error = "Writing is not supported.";

		/// <inheritdoc/>
		public TValue Get([NotNull] TKey key) => InnerDatabase.Get(key);

		/// <inheritdoc/>
		public bool TryGet([NotNull] TKey key, [CanBeNull] out TValue value) => InnerDatabase.TryGet(key, out value);

		/// <inheritdoc/>
		public IEnumerable<ILazyLoader<TValue>> GetAll([NotNull] TKey key) => InnerDatabase.GetAll(key);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<TKey, ILazyLoader<TValue>>> GetEnumerator() => InnerDatabase.GetEnumerator();

		/// <inheritdoc/>
		public void Insert([NotNull] TKey key, [NotNull] TValue value) => throw new NotSupportedException(Error);

		/// <inheritdoc/>
		public void InsertRange([NotNull] params KeyValuePair<TKey, TValue>[] items) => throw new NotSupportedException(Error);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => InnerDatabase.GetEnumerator();
	}
}