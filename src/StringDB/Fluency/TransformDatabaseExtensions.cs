using JetBrains.Annotations;

using StringDB.Databases;

using System.Runtime.CompilerServices;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="TransformDatabase{TPreTransformKey,TPreTransformValue,TPostTransformKey,TPostTransformValue}"/>.
	/// </summary>
	[PublicAPI]
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
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		/// <returns>A <see cref="TransformDatabase{TPreTransformKey,TPreTransformValue,TPostTransformKey,TPostTransformValue}"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TPostKey, TPostValue> WithTransform<TPreKey, TPreValue, TPostKey, TPostValue>
		(
			[NotNull] this IDatabase<TPreKey, TPreValue> database,
			[NotNull] ITransformer<TPreKey, TPostKey> keyTransformer,
			[NotNull] ITransformer<TPreValue, TPostValue> valueTransformer,
			bool disposeDatabase = true
		)
			=> new TransformDatabase<TPreKey, TPreValue, TPostKey, TPostValue>
			(
				database,
				keyTransformer,
				valueTransformer,
				disposeDatabase
			);
	}
}