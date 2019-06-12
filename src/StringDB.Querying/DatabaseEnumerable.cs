using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Threading;

namespace StringDB.Querying
{
	/// <summary>
	/// Utility functions for turning a database into a train enumerable.
	/// </summary>
	[PublicAPI]
	public static class DatabaseEnumerable
	{
		/// <summary>
		/// Turns a database into a TrainEnumerable.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="database">The database to use as a source.</param>
		/// <param name="requestFactory">The factory to convert lazy loaders to <see cref="IRequest{TValue}"/>s.</param>
		/// <param name="lock">The lock to use while enumerating.</param>
		/// <returns>A <see cref="TrainEnumerable{T}"/>, capable of allowing multiple concurrent
		/// enumerators and only enumerate over the source once.</returns>
		[NotNull]
		public static TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>> MakeTrainEnumerable<TKey, TValue>
		(
			[NotNull] this IDatabase<TKey, TValue> database,
			[NotNull] Func<ILazyLoader<TValue>, IRequest<TValue>> requestFactory,
			[NotNull] SemaphoreSlim @lock
		)
			=> new TrainEnumerable<KeyValuePair<TKey, IRequest<TValue>>>
			(
				database.ModifyValue(requestFactory)
				.EnumerateWithLocking(@lock)
			);

		/// <summary>
		/// Enumerates over an enumerable, locking during all points of enumeration
		/// and unlocking to return a value.
		/// </summary>
		/// <typeparam name="T">The type of values in the source.</typeparam>
		/// <param name="source">The source enumerable to enumerate.</param>
		/// <param name="lock">The lock to lock on while </param>
		/// <returns>An enumerable, but it locks during enumeration.</returns>
		[NotNull]
		public static IEnumerable<T> EnumerateWithLocking<T>
		(
			[NotNull] this IEnumerable<T> source,
			[NotNull] SemaphoreSlim @lock
		)
		{
			@lock.WaitAsync()
				.ConfigureAwait(false);

			foreach (var kvp in source)
			{
				@lock.Release();

				yield return kvp;

				@lock.WaitAsync()
					.ConfigureAwait(false);
			}

			@lock.Release();
		}

		/// <summary>
		/// Maps one type of values into another.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The old type of value that the enumerable is.</typeparam>
		/// <typeparam name="TNewValue">The new type of value to morph into.</typeparam>
		/// <param name="source">The source enumerable, that will have the values transformed.</param>
		/// <param name="factory">The factory to use during the conversion process.</param>
		/// <returns>An enumerable of mapped values.</returns>
		[NotNull]
		public static IEnumerable<KeyValuePair<TKey, TNewValue>> ModifyValue<TKey, TValue, TNewValue>
		(
			[NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> source,
			[NotNull] Func<TValue, TNewValue> factory
		)
		{
			foreach (var kvp in source)
			{
				yield return new KeyValuePair<TKey, TNewValue>
				(
					kvp.Key,
					factory(kvp.Value)
				);
			}
		}
	}
}