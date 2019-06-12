using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public class ParallelAsyncController<TResult>
	{
		private readonly CancellationTokenSource _cancellationTokenSource;

		public ParallelAsyncController(CancellationTokenSource cancellationTokenSource)
		{
			_cancellationTokenSource = cancellationTokenSource;
			Result = default;
		}

		public void Stop()
		{
			_cancellationTokenSource.Cancel();
		}

		public TResult Result { get; private set; }

		public void ProvideResult(TResult result)
		{
			Result = result;
		}
	}
}