using JetBrains.Annotations;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	[PublicAPI]
	public sealed class ChannelMessagePipe<T> : IMessagePipe<T>
	{
		public const int DefaultMaxCapacity = 0x1000;
		public const int Unbounded = -1;

		private readonly Channel<T> _channel;

		public ChannelMessagePipe(int maxCapacity = DefaultMaxCapacity)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Enqueue([NotNull] T message)
		{
			// will only be false if the channel closed
			_channel.Writer.TryWrite(message);
		}

		public ValueTask<T> Dequeue(CancellationToken cancellationToken = default)
		{
			if (_channel.Reader.TryRead(out var item))
			{
				return new ValueTask<T>(item);
			}

			return AsyncDequeue(cancellationToken);
		}

		private async ValueTask<T> AsyncDequeue(CancellationToken cancellationToken)
		{
			while (await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
			{
				if (_channel.Reader.TryRead(out var item))
				{
					return item;
				}
			}

			ThrowObjectDisposedException();

			// won't execute
			return default;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowObjectDisposedException()
		{
			throw new ObjectDisposedException(nameof(ChannelMessagePipe<T>));
		}

		public void Dispose()
		{
			_channel.Writer.Complete();
		}
	}
}