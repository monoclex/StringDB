using Ceras;

using StringDB.Databases;
using StringDB.Fluency;

using System.Runtime.CompilerServices;

namespace StringDB.Ceras
{
	/// <summary>
	/// Fluent extensions to apply to byte array based databases.
	/// </summary>
	public static class CerasTransformerExtensions
	{
		public static CerasSerializer CerasInstance { get; } = new CerasSerializer();

		/// <summary>
		/// Use Ceras with a database. Uses <see cref="CerasInstance"/> as a serializer.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="database">The database to apply Ceras to.</param>
		/// <returns>A <see cref="TransformDatabase{TPreKey,TPreValue,TPostKey,TPostValue}"/> that has a <see cref="CerasTransformer{T}"/> applied to it.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TKey, TValue> WithCeras<TKey, TValue>
		(
			this IDatabase<byte[], byte[]> database
		)
			=> database.WithCeras<TKey, TValue>(CerasInstance);

		/// <summary>
		/// Use Ceras with a database.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="database">The database to use.</param>
		/// <param name="serializer">The serializer to use.</param>
		/// <returns>A <see cref="TransformDatabase{TPreKey,TPreValue,TPostKey,TPostValue}"/> that has a <see cref="CerasTransformer{T}"/> applied to it.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TKey, TValue> WithCeras<TKey, TValue>
		(
			this IDatabase<byte[], byte[]> database,
			CerasSerializer serializer
		)
			=> database.WithTransform
			(
				new CerasTransformer<TKey>(serializer),
				new CerasTransformer<TValue>(serializer)
			);
	}
}