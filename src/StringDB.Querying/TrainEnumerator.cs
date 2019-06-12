using System;
using System.Collections;
using System.Collections.Generic;

namespace StringDB.Querying
{
	/// <summary>
	/// The enumerator for a <see cref="TrainEnumerable{T}"/>.
	/// This will get values from a parent, and kill itself when
	/// it realizes it's back where it first spawned off of the
	/// parent.
	/// </summary>
	/// <typeparam name="T">The type of source values.</typeparam>
	public class TrainEnumerator<T> : IEnumerator<T>
	{
		// the parent.
		private readonly TrainEnumerable<T> _parent;

		// where it spawned off of the train parent from.
		private readonly int _current;

		// index in the train cache.
		private long _index;

		// if the enumerator has been enumerated at least once.
		// this is to prevent infant death syndrome from thinking
		// that it is back where it started, when it has really
		// only just begun
		private bool _enumerated;

		public TrainEnumerator(TrainEnumerable<T> parent, int current, long index)
		{
			_parent = parent;
			_current = current == -1 ? 0 : current;
			_index = index;
		}

		public T Current { get; private set; }

		object IEnumerator.Current => Current;

		public void Dispose() => _parent.BeheadChild();

		public bool MoveNext()
		{
			var result = _parent.Next(_index++, out var current);
			Current = current;

			// stop at where we were picked up
			if (_current == _parent.Current
				&& _enumerated)
			{
				return false;
			}

			_enumerated = true;

			return result;
		}

		// lmao nothing implements/uses this anyway
		public void Reset() => throw new NotSupportedException();
	}
}