using JetBrains.Annotations;
using StringDB.Querying.Messaging;

using System;
using System.Collections.Generic;
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
	public sealed class PipeRequest<TRequestKey, TValue> : IRequest<TValue>
	{
		[NotNull] private readonly CancellationTokenSource _cts;
		[NotNull] private readonly Task<TValue> _requestTask;
		[NotNull] private readonly TRequestKey _requestKey;
		[NotNull] private readonly IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> _requestPipe;

		public PipeRequest
		(
			[NotNull] IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> requestPipe,
			[NotNull] IMessagePipe<TValue> valuePipe,
			[NotNull] TRequestKey requestKey
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

		[NotNull]
		public IMessagePipe<TValue> ValuePipe { get; }

		public bool HasAnswer { get; set; }

		[NotNull, ItemNotNull]
		public ValueTask<TValue> Request()
		{
			_requestPipe.Enqueue(new KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>
			(
				_requestKey,
				this
			));

			return new ValueTask<TValue>(_requestTask);
		}

		public void Dispose()
		{
			_cts.Cancel();
			ValuePipe.Dispose();
		}
	}
}