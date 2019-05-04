using JetBrains.Annotations;

using StringDB.LazyLoaders;

using System.Collections.Generic;
using System.Linq;

namespace StringDB.Databases
{
	/// <inheritdoc />
	/// <summary>
	/// A database that uses <see cref="T:StringDB.ITransformer`2" />s to transform
	/// keys and values to/from the underlying database.
	/// </summary>
	/// <typeparam name="TPreKey">The key type before transformation.</typeparam>
	/// <typeparam name="TPreValue">The value type before transformation.</typeparam>
	/// <typeparam name="TPostKey">The key type after transformation.</typeparam>
	/// <typeparam name="TPostValue">The value type after transformation</typeparam>
	[PublicAPI]
	public sealed class TransformDatabase<TPreKey, TPreValue, TPostKey, TPostValue>
		: BaseDatabase<TPostKey, TPostValue>, IDatabaseLayer<TPreKey, TPreValue>
	{
		private readonly ITransformer<TPreKey, TPostKey> _keyTransformer;
		private readonly ITransformer<TPreValue, TPostValue> _valueTransformer;
		private readonly bool _disposeDatabase;

		/// <inheritdoc />
		public IDatabase<TPreKey, TPreValue> InnerDatabase { get; }

		/// <summary>
		/// Create a new transform database.
		/// </summary>
		/// <param name="db">The underlying database to convert.</param>
		/// <param name="keyTransformer">The transformer for the key.</param>
		/// <param name="valueTransformer">The transformer for the value.</param>
		public TransformDatabase
		(
			[NotNull] IDatabase<TPreKey, TPreValue> db,
			[NotNull] ITransformer<TPreKey, TPostKey> keyTransformer,
			[NotNull] ITransformer<TPreValue, TPostValue> valueTransformer,
			bool disposeDatabase = true
		)
			: this(db, keyTransformer, valueTransformer, EqualityComparer<TPostKey>.Default, disposeDatabase)
		{
		}

		/// <summary>
		/// Create a new transform database.
		/// </summary>
		/// <param name="db">The underlying database to convert.</param>
		/// <param name="keyTransformer">The transformer for the key.</param>
		/// <param name="valueTransformer">The transformer for the value.</param>
		/// <param name="comparer">The equality comparer to use for keys.</param>
		public TransformDatabase
		(
			[NotNull] IDatabase<TPreKey, TPreValue> db,
			[NotNull] ITransformer<TPreKey, TPostKey> keyTransformer,
			[NotNull] ITransformer<TPreValue, TPostValue> valueTransformer,
			[NotNull] IEqualityComparer<TPostKey> comparer,
			bool disposeDatabase = true
		)
			: base(comparer)
		{
			InnerDatabase = db;
			_keyTransformer = keyTransformer;
			_valueTransformer = valueTransformer;
			_disposeDatabase = disposeDatabase;
		}

		/// <inheritdoc />
		public override void InsertRange(params KeyValuePair<TPostKey, TPostValue>[] items)
		{
			var pre = new KeyValuePair<TPreKey, TPreValue>[items.Length];

			for (var i = 0; i < items.Length; i++)
			{
				var current = items[i];

				pre[i] = new KeyValuePair<TPreKey, TPreValue>
				(
					key: _keyTransformer.TransformPost(current.Key),
					value: _valueTransformer.TransformPost(current.Value)
				);
			}

			InnerDatabase.InsertRange(pre);
		}

		/// <inheritdoc />
		protected override IEnumerable<KeyValuePair<TPostKey, ILazyLoader<TPostValue>>> Evaluate()
			=> InnerDatabase
			.Select
			(
				x => new KeyValuePair<TPostKey, ILazyLoader<TPostValue>>
				(
					key: _keyTransformer.TransformPre(x.Key),
					value: new TransformLazyLoader<TPreValue, TPostValue>(x.Value, _valueTransformer)
				)
			);

		/// <inheritdoc />
		public override void Dispose()
		{
			if (_disposeDatabase)
			{
				InnerDatabase.Dispose();
			}
		}
	}
}