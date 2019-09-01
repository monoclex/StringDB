using JetBrains.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Messaging
{
	/// <summary>
	/// An implementation of <see cref="IMessagePipe{T}"/> that allows for
	/// a single Enqueue call to be passed to multiple message pipes.
	/// Used primarily for an IIterationManager to pass a single message
	/// along multiple chains.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class BranchMessagePipe<T> : IMessagePipe<T>
	{
		[NotNull, ItemNotNull]
		private IMessagePipe<T>[] _pipes = new IMessagePipe<T>[0];

		[NotNull, ItemNotNull]
		public IMessagePipe<T>[] Pipes => _pipes;

		public void AddRange([NotNull, ItemNotNull] IMessagePipe<T>[] messagePipes)
		{
			var newPipes = new IMessagePipe<T>[_pipes.Length + messagePipes.Length];

			Array.Copy(_pipes, 0, newPipes, 0, _pipes.Length);
			Array.Copy(messagePipes, 0, newPipes, _pipes.Length, messagePipes.Length);

			_pipes = newPipes;
		}

		public void Add([NotNull] IMessagePipe<T> messagePipe) => AddRange(new IMessagePipe<T>[1] { messagePipe });

		public void Remove([NotNull] IMessagePipe<T> pipe)
		{
			var copy = _pipes;
			var index = Array.IndexOf(copy, pipe);

			if (index == -1)
			{
				throw new ArgumentException("The pipe doesn't exist in the pipes", nameof(pipe));
			}

			var newPipes = new IMessagePipe<T>[copy.Length - 1];

			// copy is 7
			// idnex is 5
			// newPipes is 2

			Array.Copy(copy, 0, newPipes, 0, index);
			Array.Copy(copy, index + 1, newPipes, index, copy.Length - index - 1);

			_pipes = newPipes;
		}

		public void Enqueue([NotNull] T message)
		{
			// create a local reference to _pipes since it could be changed later.
			var copy = _pipes;

			for (var i = 0; i < copy.Length; i++)
			{
				var pipe = copy[i];

				pipe.Enqueue(message);
			}
		}

		public ValueTask<T> Dequeue(CancellationToken cancellationToken = default)
			=> throw new InvalidOperationException();

		public void Dispose()
		{
			// idk
			var copy = _pipes;

			foreach (var element in copy)
			{
				element.Dispose();
			}
		}
	}
}