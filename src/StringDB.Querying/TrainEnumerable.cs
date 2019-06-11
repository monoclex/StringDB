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
		}

		private ManualResetEventSlim _mres = new ManualResetEventSlim();
		private object _lock = new object();
		private int _children;
		private bool _areWaiters;

		public int Current { get; private set; }

		public int _nextRequests;

		private bool _doNext;
		private T _next;

		private bool ActualNext()
		{
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
					return false;
				}
			}

			_next = _enumerator.Current;
			Current++;

			return true;
		}

		// we will wait for all train enumerators to request the
		// next one or die.
		public bool Next(out T next)
		{
			lock (_lock)
			{
				_areWaiters = true;

				_nextRequests++;

				if (_nextRequests >= _children)
				{
					_nextRequests = 0;

					// first compute the next value
					_doNext = ActualNext();

					// then let the waiting threads go
					_mres.Set();
					goto @afterWait;
				}
			}

			_mres.Wait();

			// just one thread should do the un-doing

			bool aquired = false;
			try
			{
				Monitor.TryEnter(_lock, ref aquired);

				if (aquired)
				{
					_mres.Reset();
					_areWaiters = false;
				}
			}
			finally
			{
				if (aquired)
				{
					Monitor.Exit(_lock);
				}
			}

		@afterWait:

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
			lock (_lock)
			{
				_children--;

				// we don't add 1 to next requests even though we will in the lock
				// because we should only go IF the condition should be passed without us
				// that means that the beheading of this child is the reason all
				// the train cars can't move, and we are responsible for that. Thus,
				// we must force them all to go.

				if (_areWaiters
					&& _nextRequests >= _children)
				{
					// call upon Next incase they were waiting for someone to go next
					// this should execute immediately
					// NOTE: there is no deadlocking despite the two lock statements
					Next(out _);
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			lock (_lock)
			{
				_children++;

				return new TrainEnumerator<T>(this, Current);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}