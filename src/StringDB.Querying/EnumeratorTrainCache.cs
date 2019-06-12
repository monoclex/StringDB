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
}