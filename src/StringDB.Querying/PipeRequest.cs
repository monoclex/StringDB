using JetBrains.Annotations;

using StringDB.Querying.Messaging;

using System.Collections.Generic;
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
		[NotNull] private readonly BackgroundTask<TValue> _request;
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

			_request = new BackgroundTask<TValue>(async (cts) =>
			{
				var value = await ValuePipe.Dequeue(cts).ConfigureAwait(false);

				cts.ThrowIfCancellationRequested();

				// this should be set by the PipeRequestManager *before* the value gets queued
				// HasAnswer = true;

				return value;
			});
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

			return new ValueTask<TValue>(_request.Task);
		}

		public void Dispose()
		{
			_request.Dispose();
			ValuePipe.Dispose();
		}
	}
}