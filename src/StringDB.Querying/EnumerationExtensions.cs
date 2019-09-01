using JetBrains.Annotations;
using System.Collections.Generic;

namespace StringDB.Querying
{
	/// <summary>
	/// Simple extensions for enumeration.
	/// </summary>
	[PublicAPI]
	public static class EnumerationExtensions
	{
		/// <summary>
		/// Locks on the object for each move next, to guarentee that there is
		/// only ever one thing enumerating over the IEnumerable.
		/// </summary>
		/// <typeparam name="T">The type of enumerable.</typeparam>
		/// <param name="enumerable">The enumerable to lock on.</param>
		/// <param name="lock">The object to lock on when enumerating.</param>
		[NotNull]
		public static IEnumerable<T> LockWhenEnumerating<T>
		(
			[NotNull] this IEnumerable<T> enumerable,
			[NotNull] object @lock
		)
		{
			// we can't use a foreach loop, so we'll have to get a bit dirty
			using (var enumerator = enumerable.GetEnumerator())
			{
				while (true)
				{
					// lock while enumerating
					lock (@lock)
					{
						if (!enumerator.MoveNext())
						{
							yield break;
						}
					}

					// .Current in `yield` Enumerables is thread safe, so we'll assume that's always the case.
					yield return enumerator.Current;
				}
			}
		}
	}
}