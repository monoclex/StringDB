using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace StringDB.Querying
{
	public class EnumeratorTrainCache<T>
	{
		private ConcurrentDictionary<long, TrainCache<T>> _cache = new ConcurrentDictionary<long, TrainCache<T>>();

		public int Participants;

		public long Last;

		private object _top = new object();
		public long Top;

		public T this[long index]
		{
			get
			{
				var trainCache = _cache[index];

				lock (trainCache.Lock)
				{
					trainCache.Accessors++;

					if (trainCache.Accessors >= Participants)
					{
						Interlocked.Increment(ref Last);
						_cache.TryRemove(index, out _);
					}

					return trainCache.Item;
				}
			}
		}

		public T Get(long index, Func<T> factory)
		{
			if (index >= Top)
			{
				lock (_top)
				{
					// if the index is still lower than the top
					if (index >= Top)
					{
						AppendItem(factory());
					}
				}
			}

			return this[index];
		}

		public void InviteParticipant()
		{
			Interlocked.Increment(ref Participants);
		}

		public void ExitParticipant()
		{
			Interlocked.Decrement(ref Participants);
		}

		public void AppendItem(T item)
		{
			var trainCache = new TrainCache<T>
			{
				Lock = new object(),
				Accessors = 0,
				Item = item,
			};

			lock (_top)
			{
				_cache[Top] = trainCache;
				Top++;
			}
		}
	}

	public class TrainCache<T>
	{
		public object Lock;
		public int Accessors;
		public T Item;
	}
	// TODO: big time clean

	/// <summary>
	/// All aboard the train enumerable! Once you get on the train,
	/// you'll be at any part in the enumerable, but you'll get off
	/// exactly where you started. You can pass in an initial enumerable
	/// and expect to only be enumerating over it once, while multiple
	/// threads can be looping over the train enumerable.
	/// </summary>
	/// <typeparam name="T">The type of values in the enumerable.</typeparam>
	public class TrainEnumerable<T> : IEnumerable<T>
	{
		private readonly IEnumerable<T> _enumerable;
		private IEnumerator<T> _enumerator;
		private EnumeratorTrainCache<T> _trainCache;

		public TrainEnumerable(IEnumerable<T> enumerable)
		{
			_trainCache = new EnumeratorTrainCache<T>();
			_enumerable = enumerable;
			_enumerator = _enumerable.GetEnumerator();
		}

		public int Current { get; private set; } = -1;

		private bool _doNext;
		private T _next;

		private bool ActualNext(out T result)
		{
			Current++;

			// are we at the end of the current enumerator?
			if (!_enumerator.MoveNext())
			{
				// yeah, let's get a new one
				_enumerator = _enumerable.GetEnumerator();
				Current = 0;

				// make sure that there are items in this one
				if (!_enumerator.MoveNext())
				{
					// uh oh
					result = default;
					return false;
				}
			}

			result = _enumerator.Current;

			return true;
		}

		private object _nextLock = new object();

		// we will wait for all train enumerators to request the
		// next one or die.
		public bool Next(long index, out T next)
		{
			next = _trainCache.Get(index, () =>
			{
				_doNext = ActualNext(out _next);
				return _next;
			});

			return _doNext;
		}

		public void BeheadChild()
		{
			_trainCache.ExitParticipant();
		}

		public IEnumerator<T> GetEnumerator()
		{
			_trainCache.InviteParticipant();
			return new TrainEnumerator<T>(this, Current, _trainCache.Last);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}