using System.Runtime.CompilerServices;
using System.Threading;

namespace StringDB.Querying
{
	/// <summary>
	/// A small, fast and light lock for nearly atmoic operations
	/// between two parallel workers.
	/// </summary>
	public class LightLock
	{
		private bool _request;
		private bool _notified;
		private SpinWait _caller;
		private SpinWait _worker;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Request()
		{
			// signal a request and wait to be notified
			_request = true;

			while (!_notified)
			{
				_caller.SpinOnce();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Release()
		{
			// set request to true and notified to false
			// then check once the worker sets request to false

			_request = true;
			_notified = false; // must be 2nd

			while (_request)
			{
				_caller.SpinOnce();
			}

			_caller.Reset();
		}

		/// <summary>
		/// Called by the worker thread.
		/// This will relinquish control if needed,
		/// but if control is not needed it will exit immediately.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Relinquish()
		{
			if (!_request)
			{
				return;
			}

			// to display that we have noticed the change,
			// we will set notified to true AFTER resetting
			// request.

			_request = false;
			_notified = true; // must be 2nd

			// now we wait for notified to become false

			while (_notified)
			{
				_worker.SpinOnce();
			}

			_worker.Reset();

			// now the caller has set request to true - we will update that here
			_request = false;
		}
	}
}