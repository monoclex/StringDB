using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace StringDB.Querying
{
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

		public TrainEnumerable(IEnumerable<T> enumerable)
		{
			_enumerable = enumerable;
			_enumerator = _enumerable.GetEnumerator();
			_barrier = new Barrier(0, _ => _doNext = ActualNext(out _next));
		}

		private readonly Barrier _barrier;

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

		// we will wait for all train enumerators to request the
		// next one or die.
		public bool Next(out T next)
		{
			_barrier.SignalAndWait();

			if (!_doNext)
			{
				next = default;
				return false;
			}

			next = _next;
			return true;
		}

		public void BeheadChild()
		{
			_barrier.RemoveParticipant();
		}

		public IEnumerator<T> GetEnumerator()
		{
			_barrier.AddParticipant();
			return new TrainEnumerator<T>(this, Current);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}