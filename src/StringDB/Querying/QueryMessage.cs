using StringDB.Querying.Messaging;
using StringDB.Querying.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public struct QueryMessage<TKey, TValue>
	{
		public KeyValuePair<TKey, ILazyLoader<TValue>> KeyValuePair;

		public TValue Value;

		/// <summary>
		/// When sent to a client, this will determine if <see cref="Value"/>
		/// has a value. When the query manager receives this as true from a client,
		/// this is a request for a value.
		/// </summary>
		public bool HasValue;

		public int Id;

		public bool Stop;

		public bool Go;
	}
}