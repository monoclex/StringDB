using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringDB.Tests
{
	public class MockDatabase : IDatabase<string, int>, IMockDatabase<string, int>
	{
		public List<KeyValuePair<string, LazyItem<int>>> Data { get; } =
			new List<KeyValuePair<string, LazyItem<int>>>
		(
			collection: new KeyValuePair<string, int>[]
			{
					// starts at 0 :^)
					// TAKE NOTE PROGRAMMERS!
					new KeyValuePair<string, int>("a", 0 ),
					new KeyValuePair<string, int>("b", 1 ),
					new KeyValuePair<string, int>("c", 2 ),
					new KeyValuePair<string, int>("d", 3 ),
					new KeyValuePair<string, int>("a", 4 ),
					new KeyValuePair<string, int>("b", 5 ),
					new KeyValuePair<string, int>("c", 6 ),
					new KeyValuePair<string, int>("d", 7 ),
					new KeyValuePair<string, int>("a", 8 ),
					new KeyValuePair<string, int>("b", 9 )
				}
				.Select(x => new KeyValuePair<string, LazyItem<int>>
					(
						key: x.Key,
						value: new LazyItem<int>(x.Value)
					)
				)
			);

		public IEnumerable<KeyValuePair<string, ILazyLoading<int>>> Evaluate()
		{
			foreach (var item in Data)
			{
				yield return new KeyValuePair<string, ILazyLoading<int>>(item.Key, item.Value);
			}
		}

		public int Get(string key) => Data.First(x => x.Key == key).Value.Load();

		public IEnumerable<ILazyLoading<int>> GetAll(string key) =>
			Data.Where(x => x.Key == key).Select(x => x.Value);

		public IEnumerator<KeyValuePair<string, ILazyLoading<int>>> GetEnumerator() => Evaluate().GetEnumerator();

		public void Insert(string key, int value) => Data.Add(new KeyValuePair<string, LazyItem<int>>(key, new LazyItem<int>(value)));

		public void InsertRange(KeyValuePair<string, int>[] items)
		{
			foreach (var item in items)
			{
				Insert(item.Key, item.Value);
			}
		}

		public bool TryGet(string key, out int value)
		{
			foreach (var item in Data)
			{
				if (item.Key == key)
				{
					value = item.Value.Load();
					return true;
				}
			}

			value = default;
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Dispose() => throw new System.NotImplementedException();
	}
}