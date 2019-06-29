using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A class to wait for something to evaluate.
	/// </summary>
	public class EventWaiter
	{
		private readonly ManualResetEventSlim _mres = new ManualResetEventSlim(false);
		private readonly Func<bool> _evaluation;

		private bool _isWaiting;

		/// <param name="evaluation">The evaluation function that
		/// should return true when the event occurs.</param>
		/// <param name="longDelayCount">The amount of times a
		/// <see cref="SpinWait"/> should spin before deciding
		/// to use a <see cref="ManualResetEventSlim"/> for longer,
		/// delayed waiting.</param>
		public EventWaiter
		(
			Func<bool> evaluation,
			int longDelayCount = 100
		)
		{
			_evaluation = evaluation;
		}

		/// <summary>
		/// Signal that the event has occurred to anybody waiting.
		/// The evaluation function must evaluate to true when this
		/// is called.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Signal()
		{
			if (_isWaiting)
			{
				_mres.Set();
			}
		}

		/// <summary>
		/// Waits for the event to execute.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Wait()
		{
			var waiter = new SpinWait();

			while (!_evaluation() || waiter.Count > 100)
			{
				waiter.SpinOnce();
			}

			_isWaiting = true;

			if (_evaluation())
			{
				_isWaiting = false;
				return;
			}

			_mres.Wait();
			_mres.Reset();
		}
	}
}
