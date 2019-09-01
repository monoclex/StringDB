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
	/// A <see cref="IRequestManager{TRequestKey, TValue}"/> implemented using
	/// <see cref="IMessagePipe{T}"/>s.
	/// </summary>
	public class PipeRequestManager<TRequestKey, TValue> : IRequestManager<TRequestKey, TValue>
	{
		private readonly IMessagePipe<NextRequest<TRequestKey, TValue>> _nextRequestPipe;
		private readonly IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> _requestPipe;
		private readonly Func<IMessagePipe<TValue>> _valuePipeFactory;

		public PipeRequestManager()
			: this
		(
			new SimpleMessagePipe<NextRequest<TRequestKey, TValue>>(),
			new SimpleMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>>(),
			() => new SimpleMessagePipe<TValue>()
		)
		{
		}

		public PipeRequestManager
		(
			IMessagePipe<NextRequest<TRequestKey, TValue>> nextRequestPipe,
			IMessagePipe<KeyValuePair<TRequestKey, PipeRequest<TRequestKey, TValue>>> requestPipe,
			Func<IMessagePipe<TValue>> valuePipeFactory
		)
		{
			_nextRequestPipe = nextRequestPipe;
			_requestPipe = requestPipe;
			_valuePipeFactory = valuePipeFactory;
		}

		public IRequest<TValue> CreateRequest(TRequestKey requestKey)
		{
			return new PipeRequest<TRequestKey, TValue>
			(
				_requestPipe,
				_valuePipeFactory(),
				requestKey
			);
		}

		public async Task<NextRequest<TRequestKey, TValue>> NextRequest(CancellationToken cancellationToken = default)
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
