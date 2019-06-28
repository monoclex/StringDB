using StringDB.Querying.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public class QueryManagerClient<TKey, TValue> : IMessageClient<QueryMessage>
	{
		private readonly IDatabase<TKey, TValue> _database;
		private readonly ManagedClient<QueryMessage> _client;

		public QueryManagerClient
		(
			IDatabase<TKey, TValue> database,
			CancellationToken cancellationToken = default
		)
		{
			_database = database;
			_client = new ManagedClient<QueryMessage>(WorkerThread, cancellationToken);
		}

		private async Task WorkerThread(IMessageClient<QueryMessage> client, CancellationToken cancellationToken)
		{
			const int initialSize = 10;
			const int incrementalSize = 5;

			// TODO: move this logic out
			// for an extremely lightweight lock for clients
			bool requestingClientsLock = false;
			bool noticedClientsLockRequest = false;

			var clients = new IMessageClient<QueryMessage>[initialSize];
			var clientsCount = 0;

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
						var newClients = new IMessageClient<QueryMessage>[clientsCount + incrementalSize];
						Array.Copy(clients, 0, newClients, 0, clientsCount);
						clients = newClients;
						clientsCount += incrementalSize;
					}

					// allow this client to start receiving database reads
					if (message.Data.Go)
					{
						clients[clientsCount] = message.Sender;
						clientsCount++;
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
						var newClients = new IMessageClient<QueryMessage>[clients.Length - 1];

						Array.Copy(clients, 0, newClients, 0, senderIndex);
						Array.Copy(clients, senderIndex + 1, newClients, senderIndex, clients.Length - senderIndex - 1);

						// unfortunately we can't perform these as an atomic operation
						// so we'll have to use a very light lock

						var spinWait = new SpinWait();

						requestingClientsLock = true;

						while (!noticedClientsLockRequest)
						{
							spinWait.SpinOnce();
						}

						// so now we can guarentee control, and expect the client
						// to have set requestingClientsLock to false we will only
						// set noticedClientsLockRequest to false and set
						// requestingClientsLock to true when we are done with this operation
						// we will make sure that requestingClientsLock turns to false

						clientsCount--;
						clients = newClients;

						noticedClientsLockRequest = false;
						requestingClientsLock = true;

						// make sure the reader has listened to us
						while (requestingClientsLock)
						{
							spinWait.SpinOnce();
						}
					}
				}
			});

			// reader
			while (!cancellationToken.IsCancellationRequested)
			{
				if (requestingClientsLock)
				{
					// we set this to false because we expect the messanger task
					// to set this to true very quickly after we set
					// noticedClientsLock to true.

					requestingClientsLock = false;
					noticedClientsLockRequest = true;

					var spinWait = new SpinWait();

					// the caller should set this to true when they are done
					while (!requestingClientsLock)
					{
						spinWait.SpinOnce();
					}

					// and reset it all back to false
					requestingClientsLock = false;
				}
			}

			// make sure the other task died
			await listener;
		}

		public void Dispose() => _client.Dispose();

		public Task Queue(Message<QueryMessage> message) => _client.Queue(message);

		public Task<Message<QueryMessage>> Receive() => _client.Receive();
	}
}