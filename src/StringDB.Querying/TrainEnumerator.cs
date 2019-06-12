using System;
using System.Collections;
using System.Collections.Generic;

namespace StringDB.Querying
{
	public class TrainEnumerator<T> : IEnumerator<T>
	{
		private readonly TrainEnumerable<T> _parent;
		private int _current;
		private long _index;
		private bool _enumerated;
		private int _amt;
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

			_amt++;
			_enumerated = true;

			return result;
		}

		// lmao nothing implements/uses this anyway
		public void Reset() => throw new NotSupportedException();
	}
}