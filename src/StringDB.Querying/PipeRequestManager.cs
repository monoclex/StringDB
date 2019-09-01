using JetBrains.Annotations;
using StringDB.Querying.Messaging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A <see cref="IRequestManager{TRequestKey, TValue}"/> implemented using
	/// <see cref="IMessagePipe{T}"/>s.
	/// </summary>
	[PublicAPI]
	public sealed class PipeRequestManager<TRequestKey, TValue> : IRequestManager<TRequestKey, TValue>
	{
		[NotNull] private readonly IMessagePipe<NextRequest<TRequestKey, TValue>> _nextRequestPipe;
		[NotNull] private readonly IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> _requestPipe;
		[NotNull] private readonly Func<IMessagePipe<TValue>> _valuePipeFactory;

		public PipeRequestManager(int maxCapacity = ChannelMessagePipe<int>.DefaultMaxCapacity)
			: this
		(
			new ChannelMessagePipe<NextRequest<TRequestKey, TValue>>(maxCapacity),
			new ChannelMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>>(maxCapacity),
			() => new ChannelMessagePipe<TValue>(maxCapacity)
		)
		{
		}

		public PipeRequestManager
		(
			[NotNull] IMessagePipe<NextRequest<TRequestKey, TValue>> nextRequestPipe,
			[NotNull] IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> requestPipe,
			[NotNull] Func<IMessagePipe<TValue>> valuePipeFactory
		)
		{
			_nextRequestPipe = nextRequestPipe;
			_requestPipe = requestPipe;
			_valuePipeFactory = valuePipeFactory;
		}

		[NotNull]
		public IRequest<TValue> CreateRequest(TRequestKey requestKey)
		{
			return new PipeRequest<TRequestKey, TValue>
			(
				_requestPipe,
				_valuePipeFactory(),
				requestKey
			);
		}

		[NotNull]
		public async ValueTask<NextRequest<TRequestKey, TValue>> NextRequest(CancellationToken cancellationToken = default)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var request = await _requestPipe.Dequeue(cancellationToken).ConfigureAwait(false);

				var requestKey = request.Key;
				var pipeRequest = request.Value;

				if (pipeRequest.HasAnswer)
				{
					continue;
				}

				// since this pipe request has never had an answer, we're going to find one *now*.

				// we will expect it to have one
				pipeRequest.HasAnswer = true;

				return new NextRequest<TRequestKey, TValue>(requestKey, pipeRequest.ValuePipe.Enqueue);
			}

			cancellationToken.ThrowIfCancellationRequested();

			// shouldn't happen
			return default;
		}

		public void Dispose()
		{
			_nextRequestPipe.Dispose();
		}
	}
}