using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace StringDB.Querying
{
	/// <summary>
	/// All aboard the train enumerable! Once you get on the train,
	/// you'll be at any part in the enumerable, but you'll get off
	/// exactly where you started. You can pass in an initial enumerable
	/// and expect to only be enumerating over it once, while multiple
	/// threads can be looping over the train enumerable.
	/// </summary>
	/// <typeparam name="T">The type of values in the enumerable.</typeparam>
	public class TrainEnumerable<T> : IEnumerable<T>, IDisposable
	{
		private readonly EnumeratorTrainCache<T> _trainCache;
		private readonly IEnumerable<T> _enumerable;
		private IEnumerator<T> _enumerator;

		public TrainEnumerable(IEnumerable<T> enumerable)
		{
			_trainCache = new EnumeratorTrainCache<T>();
			_enumerable = enumerable;
			_enumerator = _enumerable.GetEnumerator();
		}

		public int Current { get; private set; } = -1;

		private bool _dead;
		private bool _doNext;
		private T _next;

		private bool ActualNext(out T result)
		{
			if (_dead)
			{
				result = default;
				return false;
			}

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

		public bool Next(BigInteger index, out T next)
		{
			next = _trainCache.Get(index, () =>
			{
				_doNext = ActualNext(out _next);
				return _next;
			});

			return _doNext;
		}

		public void BeheadChild() => _trainCache.ExitParticipant();

		public IEnumerator<T> GetEnumerator()
		{
			_trainCache.InviteParticipant();
			return new TrainEnumerator<T>(this, Current, _trainCache.Last);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Dispose() => _dead = true;
	}
}