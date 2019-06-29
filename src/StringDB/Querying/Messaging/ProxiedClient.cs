using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	public class ProxiedClient<T> : IMessageClient<T>
	{
		private readonly List<IMessageClient<T>> _proxies = new List<IMessageClient<T>>();

		public void Proxy(IMessageClient<T> client)
		{
			lock (_proxies)
			{
				_proxies.Add(client);
			}
		}

		public void Deproxy(IMessageClient<T> client)
		{
			lock (_proxies)
			{
				var index = _proxies.IndexOf(client);

				if (index == -1)
				{
					return;
				}

				_proxies.RemoveAt(index);
			}
		}

		private const string ReceiveMessage =
			"You aren't suppose to receive data from a" + nameof(ProxiedClient<T>) + "."
			+ " Use the " + nameof(Proxy) + " method to proxy a client and receive data from that instead.";

		public Task<Message<T>> Receive(CancellationToken cancellationToken) => throw new NotSupportedException(ReceiveMessage);

		public void Queue(Message<T> message)
		{
			lock (_proxies)
			{
				foreach(var proxy in _proxies)
				{
					proxy.Queue(message);
				}
			}
		}

		public void Dispose()
		{
			foreach(var proxy in _proxies)
			{
				proxy.Dispose();
			}
		}
	}
}
