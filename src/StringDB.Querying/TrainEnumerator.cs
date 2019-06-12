using System;
using System.Collections;
using System.Collections.Generic;

namespace StringDB.Querying
{
	public class TrainEnumerator<T> : IEnumerator<T>
	{
		private readonly TrainEnumerable<T> _parent;
		private int _current;

		public TrainEnumerator(TrainEnumerable<T> parent, int current)
		{
			_parent = parent;
			_current = current;
		}

		public T Current { get; private set; }

		object IEnumerator.Current => Current;

		public void Dispose() => _parent.BeheadChild();

		public bool MoveNext()
		{
			var result = _parent.Next(out var current);
			Current = current;

			// stop at where we were picked up
			if (_current == _parent.Current)
			{
				return false;
			}

			if (_current == -1)
			{
				_current = 0;
			}

			return result;
		}

		// lmao nothing implements/uses this anyway
		public void Reset() => throw new NotSupportedException();
	}
}