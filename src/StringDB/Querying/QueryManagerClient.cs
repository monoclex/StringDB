using StringDB.Querying.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public class QueryManagerClient<TKey, TValue> : IMessageClient<QueryMessage<TKey, TValue>>
	{
		private readonly IDatabase<TKey, TValue> _database;
		private readonly bool _disposeDatabase;
		private readonly ManagedClient<QueryMessage<TKey, TValue>> _client;

		public QueryManagerClient
		(
			IDatabase<TKey, TValue> database,
			CancellationToken cancellationToken = default,
			bool disposeDatabase = true
		)
		{
			_database = database;
			_disposeDatabase = disposeDatabase;
			_client = new ManagedClient<QueryMessage<TKey, TValue>>(WorkerThread, cancellationToken);
		}

		private async Task WorkerThread(IMessageClient<QueryMessage<TKey, TValue>> client, CancellationToken cancellationToken)
		{
			const int initialSize = 10;
			const int incrementalSize = 5;

			var clients = new IMessageClient<QueryMessage<TKey, TValue>>[initialSize];
			var clientsCount = 0;

			var clientLightLock = new LightLock();
			var clientWaiter = new EventWaiter(() => clientsCount != 0);

			// start off a listening thread
			var listener = Task.Run(async () =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var message = await client.Receive().ConfigureAwait(false);

					if (message.LacksData)
					{
						continue;
					}

					// resize the clients array if there's too many clients
					// allocate bigger array, copy clients to it, re-assign clients and inform size change
					if (clientsCount == clients.Length)
					{
						var newClients = new IMessageClient<QueryMessage<TKey, TValue>>[clientsCount + incrementalSize];
						Array.Copy(clients, 0, newClients, 0, clientsCount);
						clients = newClients;
						clientsCount += incrementalSize;
					}

					// allow this client to start receiving database reads
					if (message.Data.Go)
					{
						clients[clientsCount] = message.Sender;
						clientsCount++;

						clientWaiter.Signal();
					}
					else if (message.Data.Stop)
					{
						var senderIndex = Array.IndexOf(clients, message.Sender);

						if (senderIndex == -1)
						{
							// if we can't find them, there's no point to do anything
							continue;
						}

						// create new amount of clients
						var newClients = new IMessageClient<QueryMessage<TKey, TValue>>[clients.Length - 1];

						Array.Copy(clients, 0, newClients, 0, senderIndex);
						Array.Copy(clients, senderIndex + 1, newClients, senderIndex, clients.Length - senderIndex - 1);

						// unfortunately, since these two aren't completely atomic
						// we will request some quick access
						clientLightLock.Request();

						clientsCount--;
						clients = newClients;

						clientLightLock.Release();
					}
				}
			});

			// TODO: split up reader into separate class so we can swap out the reader

			// reader
			while (!cancellationToken.IsCancellationRequested)
			{
				clientWaiter.Wait();

				int id = 0;
				using (var enumerator = _database.GetEnumerator())
				{
					while (enumerator.MoveNext() && clientsCount > 0 && !cancellationToken.IsCancellationRequested)
					{
						var data = new QueryMessage<TKey, TValue>
						{
							Id = id,
							KeyValuePair = enumerator.Current
						};

						for (var clientIndex = 0; clientIndex < clientsCount; clientIndex++)
						{
							client.Send(clients[clientIndex], data);
						}

						clientLightLock.Relinquish();

						id++;
					}
				}
			}

			// make sure the other task died
			await listener;
		}

		public void Dispose()
		{
			_client.Dispose();

			if (_disposeDatabase)
			{
				_database.Dispose();
			}
		}

		public void Queue(Message<QueryMessage<TKey, TValue>> message) => _client.Queue(message);

		public Task<Message<QueryMessage<TKey, TValue>>> Receive() => _client.Receive();
	}
}