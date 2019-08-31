using StringDB.Querying.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.PerformanceNumbers
{
	public class MessagePipeMessagesPerSecond
	{
		private readonly IMessagePipe<int> _pipe = new SimpleMessagePipe<int>();

		public async Task Run()
		{
			var messagesPerSecond = 0;

			var tell = Task.Run(() =>
			{
				Console.WriteLine("Tell started");
				while(true)
				{
					System.Threading.Thread.Sleep(1000);
					var loc = messagesPerSecond;
					messagesPerSecond = 0;
					Console.WriteLine($"{nameof(SimpleMessagePipe<int>)} messages per second: {loc:n0}");
				}
			});

			var push = Task.Run(() =>
			{
				Console.WriteLine("Push started");
				for (var i = int.MinValue; i < int.MaxValue; i++)
				{
					_pipe.Enqueue(i);
				}
			});

			var consume = ((Func<Task>)(async () =>
			{
				Console.WriteLine("Consume started");
				while (true)
				{
					try
					{
						var message = await _pipe.Dequeue();
						messagesPerSecond++;
					} catch (Exception ex)
					{
						Console.WriteLine("consuming ex: " + ex);
					}
				}
			}))();

			await Task.WhenAll(tell, push, consume).ConfigureAwait(false);
		}
	}
}