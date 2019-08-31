using FluentAssertions;
using StringDB.Querying.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.Querying.Messaging
{
	public class SimpleMessagePipeTests : IDisposable
	{
		[Fact]
		public async Task AfterEnqueueing_CanDequeueImmediately()
		{
			var message = new Message { Id = 5 };

			new Thread(() =>
			{
				Thread.Sleep(1000);
				Assert.True(false, "AfterEnqueueing_CanDequeueImmediately took too long to complete.");
			}).Start();

			_pipe.Enqueue(message);

			var dequeuedMessage = await _pipe.Dequeue().ConfigureAwait(false);

			dequeuedMessage.Should().BeEquivalentTo(message);
		}

		[Fact]
		public async Task DequeueCall_Completes_WhenQueueHasItems()
		{
			var message = new Message { Id = 5 };

			var dequeueCall = ((Func<Task>)(async () =>
			{
				var localMessage = await _pipe.Dequeue().ConfigureAwait(false);

				localMessage.Should().BeEquivalentTo(message);
			}))();

			_pipe.Enqueue(message);

			await dequeueCall.ConfigureAwait(false);
		}

		public class Message : IEquatable<Message>
		{
			public int Id { get; set; }

			public bool Equals(Message other) => Id == other.Id;

			public static bool operator ==(Message a, Message b) => a.Equals(b);
			public static bool operator !=(Message a, Message b) => !(a == b);
		}

		private readonly IMessagePipe<Message> _pipe = new SimpleMessagePipe<Message>();

		public void Dispose()
		{
			_pipe.Dispose();
		}
	}
}
