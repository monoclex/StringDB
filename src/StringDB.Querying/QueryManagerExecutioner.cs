using JetBrains.Annotations;
using StringDB.Querying.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public sealed class QueryManagerExecutioner<TKey, TValue> : IDisposable
	{
		private readonly IIterationManager<TKey, TValue> _iterationManager;
		private readonly BranchMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> _channel = new BranchMessagePipe<KeyValuePair<TKey, IRequest<TValue>>>();
		private readonly ManualResetEventSlim _mres = new ManualResetEventSlim();
		private BackgroundTask _channelWorker;

		public QueryManagerExecutioner
		(
			[NotNull] IDatabase<TKey, TValue> database,
			[NotNull] IRequestManager<ILazyLoader<TValue>, TValue> requestManager,
			[NotNull] IIterationManager<TKey, TValue> iterationManager
		)
		{
			_iterationManager = iterationManager;
			Current = new IterationHandle(null, () => 0, () => true);

			_channelWorker = new BackgroundTask(async (ct) =>
			{
				await Task.Yield();

				while (!ct.IsCancellationRequested)
				{
					if (MayBeginAgain && _channel.Pipes.Length > 0 && Current.AtEnd)
					{
						// begin iterating again
						Begin();
					}

					_mres.Wait(ct);
					_mres.Reset();
				}
			});
			_requestManager = requestManager;
			Database = database;
		}

		private readonly object _controlLock = new object();

		public IDisposable GainFullControl()
		{
			lock (_controlLock)
			{
				return _requestManager.Disable();
			}
		}

		[NotNull]
		public IIterationHandle Current { get; private set; }

		private bool _may = true;
		private readonly IRequestManager<ILazyLoader<TValue>, TValue> _requestManager;

		public bool MayBeginAgain
		{
			get => _may;
			set
			{
				_may = value;

				_mres.Set();
			}
		}

		public IDatabase<TKey, TValue> Database { get; }

		public void Begin()
		{
			Current = _iterationManager.IterateTo(_channel);
		}

		public void Attach([NotNull] IMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> pipe, out int index)
		{
			_channel.Add(pipe);

			// we want to set the index after we have the pipe so that way the pipe may have some
			// items before the index, but we can be sure we'll iterate through all the items
			index = Current.Index;

			_mres.Set();
		}

		public void Detach([NotNull] IMessagePipe<KeyValuePair<TKey, IRequest<TValue>>> pipe)
		{
			_channel.Remove(pipe);

			_mres.Set();
		}

		public void Dispose()
		{
			// quit the background worker
			_channelWorker.CancellationTokenSource.Cancel();
			_channelWorker.Dispose();

			// then, dispose all the pipes
			_channel.Dispose();

			// there are no more users - dispose anything else
			_mres.Dispose();
			Current.Dispose();
		}
	}
}
