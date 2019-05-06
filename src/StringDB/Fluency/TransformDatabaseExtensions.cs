using JetBrains.Annotations;

using StringDB.Databases;
using StringDB.Transformers;

using System.Runtime.CompilerServices;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="TransformDatabase{TPreKey,TPreValue,TPostKey,TPostValue}"/>.
	/// </summary>
	[PublicAPI]
	public static class TransformDatabaseExtensions
	{
		/// <summary>
		/// Applies a transformation to a database.
		/// </summary>
		/// <typeparam name="TPreKey">The database's key type.</typeparam>
		/// <typeparam name="TPreValue">The database's value type.</typeparam>
		/// <typeparam name="TPostKey">The new type of key for the database.</typeparam>
		/// <typeparam name="TPostValue">The new type of value for the database.</typeparam>
		/// <param name="database">The database to transform.</param>
		/// <param name="keyTransformer">A transformer for the keys.</param>
		/// <param name="valueTransformer">A transformer for the values.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		/// <returns>A <see cref="TransformDatabase{TPreKey,TPreValue,TPostKey,TPostValue}"/>.</returns>
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

		/// <summary>
		/// Apply a key-only transformation to a database.
		/// </summary>
		/// <typeparam name="TPreKey">The database's key type.</typeparam>
		/// <typeparam name="TPostKey">The new type of key for the database.</typeparam>
		/// <typeparam name="TValue">The database's value type.</typeparam>
		/// <param name="database">The database to transform.</param>
		/// <param name="keyTransformer">A transformer for the keys.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		/// <returns>A <see cref="TransformDatabase{TPreKey,TPreValue,TPostKey,TPostValue}"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TPostKey, TValue> WithKeyTransform<TPreKey, TPostKey, TValue>
		(
			[NotNull] this IDatabase<TPreKey, TValue> database,
			[NotNull] ITransformer<TPreKey, TPostKey> keyTransformer,
			bool disposeDatabase = true
		)
			=> database.WithTransform
			(
				keyTransformer,
				NoneTransformer<TValue>.Default,
				disposeDatabase
			);

		/// <summary>
		/// Applies a value-only transformation to a database.
		/// </summary>
		/// <typeparam name="TKey">The database's key type.</typeparam>
		/// <typeparam name="TPreValue">The database's value type.</typeparam>
		/// <typeparam name="TPostValue">The new type of value for the database.</typeparam>
		/// <param name="database">The database to transform.</param>
		/// <param name="valueTransformer">A transformer for the values.</param>
		/// <param name="disposeDatabase">If the underlying database should be disposed on dispose.</param>
		/// <returns>A <see cref="TransformDatabase{TPreKey,TPreValue,TPostKey,TPostValue}"/>.</returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TKey, TPostValue> WithValueTransform<TKey, TPreValue, TPostValue>
		(
			[NotNull] this IDatabase<TKey, TPreValue> database,
			[NotNull] ITransformer<TPreValue, TPostValue> valueTransformer,
			bool disposeDatabase = true
		)
			=> database.WithTransform
			(
				NoneTransformer<TKey>.Default,
				valueTransformer,
				disposeDatabase
			);
	}
}