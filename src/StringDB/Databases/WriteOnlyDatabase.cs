using JetBrains.Annotations;

using System;
using System.Collections.Generic;

namespace StringDB.Databases
{
	/// <summary>
	/// A database which can only be read from.
	/// </summary>
	[PublicAPI]
	public class WriteOnlyDatabase<TKey, TValue> : BaseDatabase<TKey, TValue>
	{
		[NotNull] private readonly IDatabase<TKey, TValue> _database;
		private readonly bool _disposeDatabase;

		/// <summary>
		/// Creates a new <see cref="WriteOnlyDatabase{TKey, TValue}"/>.
		/// </summary>
		/// <param name="database">The database to make write only.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		public WriteOnlyDatabase([NotNull] IDatabase<TKey, TValue> database, bool disposeDatabase = true)
		{
			_disposeDatabase = disposeDatabase;
			_database = database;
		}

		/// <inheritdoc/>
		public override void Dispose()
		{
			if (_disposeDatabase)
			{
				_database.Dispose();
			}
		}

		/// <inheritdoc/>
		public override void InsertRange(params KeyValuePair<TKey, TValue>[] items)
			=> _database.InsertRange(items);

		/// <inheritdoc/>
		protected override IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>> Evaluate()
			=> throw new NotSupportedException($"Reading is not supported.");
	}
}