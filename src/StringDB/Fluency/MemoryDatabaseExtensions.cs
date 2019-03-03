using JetBrains.Annotations;

using StringDB.Databases;

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StringDB.Fluency
{
	/// <summary>
	/// Fluent extensions for a <see cref="MemoryDatabase{TKey,TValue}"/>
	/// </summary>
	[PublicAPI]
	public static class MemoryDatabaseExtensions
	{
		/// <summary>
		/// Creates a blank <see cref="MemoryDatabase{TKey,TValue}"/>
		/// </summary>
		/// <typeparam name="TKey">The type of key to use.</typeparam>
		/// <typeparam name="TValue">The type of value to use.</typeparam>
		/// <param name="builder">The builder.</param>
		/// <returns>A <see cref="MemoryDatabase{TKey,TValue}"/></returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TKey, TValue> UseMemoryDatabase<TKey, TValue>
		(
			[CanBeNull] this DatabaseBuilder builder
		)
			=> builder.UseMemoryDatabase<TKey, TValue>(null);

		/// <summary>
		/// Creates a <see cref="MemoryDatabase{TKey,TValue}"/> with the specified data.
		/// </summary>
		/// <typeparam name="TKey">The type of key to use.</typeparam>
		/// <typeparam name="TValue">The type of value to use.</typeparam>
		/// <param name="builder">The builder.</param>
		/// <param name="data">The data to prefill it with.</param>
		/// <returns>A <see cref="MemoryDatabase{TKey,TValue}"/></returns>
		[NotNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDatabase<TKey, TValue> UseMemoryDatabase<TKey, TValue>
		(
			[CanBeNull] this DatabaseBuilder builder,
			[NotNull] List<KeyValuePair<TKey, TValue>> data
		)
			=> new MemoryDatabase<TKey, TValue>(data);
	}
}