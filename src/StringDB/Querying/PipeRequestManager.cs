using StringDB.Querying.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private readonly IMessagePipe<TRequestKey> _requestPipe;

		public PipeRequestManager
		(
			IMessagePipe<NextRequest<TRequestKey, TValue>> nextRequestPipe,
			IMessagePipe<TRequestKey> requestPipe
		)
		{
			_nextRequestPipe = nextRequestPipe;
			_requestPipe = requestPipe;
		}

		public IRequest<TValue> CreateRequest(TRequestKey requestKey)
		{
			return default;
		}

		public Task<NextRequest<TRequestKey, TValue>> NextRequest() => throw new NotImplementedException();

		public void Dispose()
		{
			_nextRequestPipe.Dispose();
		}
	}
}
