using StringDB.Databases;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="TransformDatabase{TPreTransformKey,TPreTransformValue,TPostTransformKey,TPostTransformValue}"/>.
	/// </summary>
	public static class TransformDatabaseExtensions
	{
		/// <summary>
		/// Apply a transformation to a database.
		/// </summary>
		/// <typeparam name="TPreKey">The database's key type.</typeparam>
		/// <typeparam name="TPreValue">The database's value type.</typeparam>
		/// <typeparam name="TPostKey">The new type of key for the database.</typeparam>
		/// <typeparam name="TPostValue">The new type of value for the database.</typeparam>
		/// <param name="database">The database to transform.</param>
		/// <param name="keyTransformer">A transformer for the keys.</param>
		/// <param name="valueTransformer">A transformer for the values.</param>
		/// <returns>A <see cref="TransformDatabase{TPreTransformKey,TPreTransformValue,TPostTransformKey,TPostTransformValue}"/>.</returns>
		public static IDatabase<TPostKey, TPostValue> WithTransform<TPreKey, TPreValue, TPostKey, TPostValue>
		(
			this IDatabase<TPreKey, TPreValue> database,
			ITransformer<TPreKey, TPostKey> keyTransformer,
			ITransformer<TPreValue, TPostValue> valueTransformer
		)
			=> new TransformDatabase<TPreKey, TPreValue, TPostKey, TPostValue>
			(
				database,
				keyTransformer,
				valueTransformer
			);
	}
}