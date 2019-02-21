using System.Collections.Generic;
using System.Linq;

namespace StringDB.Databases
{
	/// <summary>
	/// A database that uses <see cref="ITransformer{TPre, TPost}"/>s to transform
	/// keys and values to/from the underlying database.
	/// </summary>
	/// <typeparam name="TPreTransformKey">The key type before transformation.</typeparam>
	/// <typeparam name="TPreTransformValue">The value type before transformation.</typeparam>
	/// <typeparam name="TPostTransformKey">The key type after transformation.</typeparam>
	/// <typeparam name="TPostTransformValue">The value type after transformation</typeparam>
	public sealed class TransformDatabase<TPreTransformKey, TPreTransformValue, TPostTransformKey, TPostTransformValue>
		: BaseDatabase<TPostTransformKey, TPostTransformValue>
	{
		private sealed class LazyTransformingValue : ILazyLoading<TPostTransformValue>
		{
			private readonly ITransformer<TPreTransformValue, TPostTransformValue> _transformer;
			private readonly ILazyLoading<TPreTransformValue> _pre;

			public LazyTransformingValue
			(
				ILazyLoading<TPreTransformValue> pre,
				ITransformer<TPreTransformValue, TPostTransformValue> transformer
			)
			{
				_pre = pre;
				_transformer = transformer;
			}

			public TPostTransformValue Load()
			{
				var loaded = _pre.Load();

				return _transformer.TransformPre(loaded);
			}
		}

		private readonly IDatabase<TPreTransformKey, TPreTransformValue> _db;
		private readonly ITransformer<TPreTransformKey, TPostTransformKey> _keyTransformer;
		private readonly ITransformer<TPreTransformValue, TPostTransformValue> _valueTransformer;

		/// <summary>
		/// Create a new transform database.
		/// </summary>
		/// <param name="db">The underlying database to convert.</param>
		/// <param name="keyTransformer">The transformer for the key.</param>
		/// <param name="valueTransformer">The transformer for the value.</param>
		public TransformDatabase
		(
			IDatabase<TPreTransformKey, TPreTransformValue> db,
			ITransformer<TPreTransformKey, TPostTransformKey> keyTransformer,
			ITransformer<TPreTransformValue, TPostTransformValue> valueTransformer
		)
		{
			_db = db;
			_keyTransformer = keyTransformer;
			_valueTransformer = valueTransformer;
		}

		public override void InsertRange(KeyValuePair<TPostTransformKey, TPostTransformValue>[] items)
		{
			var pre = new KeyValuePair<TPreTransformKey, TPreTransformValue>[items.Length];

			for (var i = 0; i < items.Length; i++)
			{
				var current = items[i];

				pre[i] = new KeyValuePair<TPreTransformKey, TPreTransformValue>
				(
					key: _keyTransformer.TransformPost(current.Key),
					value: _valueTransformer.TransformPost(current.Value)
				);
			}

			_db.InsertRange(pre);
		}

		protected override IEnumerable<KeyValuePair<TPostTransformKey, ILazyLoading<TPostTransformValue>>> Evaluate()
			=> _db
			.Select
			(
				x => new KeyValuePair<TPostTransformKey, ILazyLoading<TPostTransformValue>>
				(
					key: _keyTransformer.TransformPre(x.Key),
					value: new LazyTransformingValue(x.Value, _valueTransformer)
				)
			);

		public override void Dispose() => _db.Dispose();
	}
}