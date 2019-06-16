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
	public sealed class WriteOnlyDatabase<TKey, TValue>
		: IDatabase<TKey, TValue>, IDatabaseLayer<TKey, TValue>
	{
		private readonly bool _disposeDatabase;

		/// <summary>
		/// Creates a new <see cref="WriteOnlyDatabase{TKey, TValue}"/>.
		/// </summary>
		/// <param name="database">The database to make write only.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		public WriteOnlyDatabase([NotNull] IDatabase<TKey, TValue> database, bool disposeDatabase = true)
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

		private const string Error = "Reading is not supported.";

		/// <inheritdoc/>
		public TValue Get([NotNull] TKey key) => throw new NotSupportedException(Error);

		/// <inheritdoc/>
		public bool TryGet([NotNull] TKey key, [CanBeNull] out TValue value) => throw new NotSupportedException(Error);

		/// <inheritdoc/>
		public IEnumerable<ILazyLoader<TValue>> GetAll([NotNull] TKey key) => throw new NotSupportedException(Error);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<TKey, ILazyLoader<TValue>>> GetEnumerator() => throw new NotSupportedException(Error);

		/// <inheritdoc/>
		public void Insert([NotNull] TKey key, [NotNull] TValue value) => InnerDatabase.Insert(key, value);

		/// <inheritdoc/>
		public void InsertRange([NotNull] params KeyValuePair<TKey, TValue>[] items) => InnerDatabase.InsertRange(items);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException(Error);
	}
}