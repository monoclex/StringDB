using JetBrains.Annotations;

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	public sealed class ChannelMessagePipe<T> : IMessagePipe<T>
	{
		public const int Unbounded = -1;

		private readonly Channel<T> _channel;

		public ChannelMessagePipe(int maxCapacity = 0x1000)
		{
			if (maxCapacity == Unbounded)
			{
				_channel = Channel.CreateUnbounded<T>();
			}
			else
			{
				_channel = Channel.CreateBounded<T>(new BoundedChannelOptions(maxCapacity)
				{
					FullMode = BoundedChannelFullMode.Wait
				});
			}
		}

		public void Enqueue([NotNull] T message)
		{
			// will only be false if the channel closed
			_channel.Writer.TryWrite(message);
		}

		public async Task<T> Dequeue(CancellationToken cancellationToken = default)
		{
			if (_channel.Reader.TryRead(out var item))
			{
				return item;
			}

			while (await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
			{
				if (_channel.Reader.TryRead(out item))
				{
					return item;
				}
			}

			throw new ObjectDisposedException(nameof(ChannelMessagePipe<T>));
		}

		public void Dispose()
		{
			_channel.Writer.Complete();
		}
	}
}