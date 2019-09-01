using StringDB.Querying.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// An implementation of <see cref="IRequest{TValue}"/> specifically for a
	/// <see cref="PipeRequestManager{TRequestKey, TValue}"/>
	/// </summary>
	/// <typeparam name="TRequestKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class PipeRequest<TRequestKey, TValue> : IRequest<TValue>
	{
		private readonly CancellationTokenSource _cts;
		private readonly Task<TValue> _requestTask;
		private readonly TRequestKey _requestKey;
		private readonly IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> _requestPipe;

		public PipeRequest
		(
			IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> requestPipe,
			IMessagePipe<TValue> valuePipe,
			TRequestKey requestKey
		)
		{
			_requestPipe = requestPipe;
			ValuePipe = valuePipe;
			_requestKey = requestKey;

			_cts = new CancellationTokenSource();

			_requestTask = ((Func<Task<TValue>>)(async () =>
			{
				var value = await ValuePipe.Dequeue(_cts.Token).ConfigureAwait(false);

				_cts.Token.ThrowIfCancellationRequested();

				// this should be set by the PipeRequestManager *before* the value gets queued
				// HasAnswer = true;

				return value;
			}))();
		}

		public IMessagePipe<TValue> ValuePipe { get; }

		public bool HasAnswer { get; set; }

		public Task<TValue> Request()
		{
			_requestPipe.Enqueue(new KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>
			(
				_requestKey,
				this
			));

			return _requestTask;
		}

		public void Dispose()
		{
			_cts.Cancel();
			ValuePipe.Dispose();
		}
	}
}
