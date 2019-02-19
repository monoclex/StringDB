using System.Collections.Generic;
using System.Linq;

namespace StringDB
{
	public interface IDatabase<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ILazyLoading<TValue>>>
	{
		TValue Get(TKey key);

		bool TryGet(TKey key, out TValue value);

		IEnumerable<ILazyLoading<TValue>> GetAll(TKey key);

		void Insert(TKey key, TValue value);

		void InsertRange(KeyValuePair<TKey, TValue>[] items);
	}

	public static class DatabaseExtensions
	{
		public static IEnumerable<TKey> Keys<TKey, TValue>(this IDatabase<TKey, TValue> db)
			=> db.Select(x => x.Key);

		public static IEnumerable<ILazyLoading<TValue>> Values<TKey, TValue>(this IDatabase<TKey, TValue> db)
			=> db.Select(x => x.Value);

		public static IEnumerable<TValue> ValuesAggresive<TKey, TValue>(this IDatabase<TKey, TValue> db)
			=> db.Values().Select(x => x.Load());

		public static IEnumerable<KeyValuePair<TKey, TValue>> EnumerateAggresively<TKey, TValue>(this IDatabase<TKey, TValue> db, int valueLoadAmount)
		{
			var lazyList = new List<KeyValuePair<TKey, ILazyLoading<TValue>>>(valueLoadAmount);
			var loadedList = new List<KeyValuePair<TKey, TValue>>(valueLoadAmount);

			using (var enumerator = db.GetEnumerator())
			{
				int result;

				do
				{
					result = Pool(valueLoadAmount, enumerator, ref lazyList);

					foreach (var item in lazyList)
					{
						loadedList.Add(new KeyValuePair<TKey, TValue>(item.Key, item.Value.Load()));
					}

					foreach (var item in loadedList)
					{
						yield return item;
					}

					loadedList.Clear();
					lazyList.Clear();
				}
				while (result == valueLoadAmount);
			}
		}

		private static int Pool<TKey, TValue>
		(
			int amount,
			IEnumerator<KeyValuePair<TKey, ILazyLoading<TValue>>> enumerator,
			ref List<KeyValuePair<TKey, ILazyLoading<TValue>>> lazyList
		)
		{
			var fillAmount = 0;

			for (; fillAmount < amount && enumerator.MoveNext(); fillAmount++)
			{
				lazyList.Add(enumerator.Current);
			}

			return fillAmount;
		}
	}
}