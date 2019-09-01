using FluentAssertions;
using JetBrains.Annotations;
using Moq;
using StringDB.Querying.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StringDB.Tests.Querying.Messaging
{
	public class BranchMessagePipeTests
	{
		private class TestPipe : IMessagePipe<int>
		{
			public TestPipe(int id) => Id = id;
			public int Id { get; set; }

			public ValueTask<int> Dequeue(CancellationToken cancellationToken = default) => throw new NotImplementedException();
			public void Dispose() => throw new NotImplementedException();
			public void Enqueue(int message) => throw new NotImplementedException();
		}

		[Fact]
		public void BranchPipe_AcceptsNewItems_AndRemovesThem_Fine()
		{
			var branchPipe = new BranchMessagePipe<int>();

			var a = new TestPipe(0);
			var b = new TestPipe(1);
			var c = new TestPipe(2);
			var d = new TestPipe(3);

			branchPipe.AddRange(new IMessagePipe<int>[] { a, b, c, d });

			branchPipe.Pipes.Length.Should().Be(4);
			branchPipe.Pipes[0].Should().Be(a);
			branchPipe.Pipes[1].Should().Be(b);
			branchPipe.Pipes[2].Should().Be(c);
			branchPipe.Pipes[3].Should().Be(d);

			branchPipe.Remove(a);

			branchPipe.Pipes.Length.Should().Be(3);
			branchPipe.Pipes[0].Should().Be(b);
			branchPipe.Pipes[1].Should().Be(c);
			branchPipe.Pipes[2].Should().Be(d);

			branchPipe.Remove(d);

			branchPipe.Pipes.Length.Should().Be(2);
			branchPipe.Pipes[0].Should().Be(b);
			branchPipe.Pipes[1].Should().Be(c);

			branchPipe.Add(a);

			branchPipe.Pipes.Length.Should().Be(3);
			branchPipe.Pipes[0].Should().Be(b);
			branchPipe.Pipes[1].Should().Be(c);
			branchPipe.Pipes[2].Should().Be(a);

			branchPipe.Add(d);

			branchPipe.Pipes.Length.Should().Be(4);
			branchPipe.Pipes[0].Should().Be(b);
			branchPipe.Pipes[1].Should().Be(c);
			branchPipe.Pipes[2].Should().Be(a);
			branchPipe.Pipes[3].Should().Be(d);

			branchPipe.Remove(c);

			branchPipe.Pipes.Length.Should().Be(3);
			branchPipe.Pipes[0].Should().Be(b);
			branchPipe.Pipes[1].Should().Be(a);
			branchPipe.Pipes[2].Should().Be(d);

			branchPipe.Add(c);

			branchPipe.Pipes.Length.Should().Be(4);
			branchPipe.Pipes[0].Should().Be(b);
			branchPipe.Pipes[1].Should().Be(a);
			branchPipe.Pipes[2].Should().Be(d);
			branchPipe.Pipes[3].Should().Be(c);

			branchPipe.Remove(d);

			branchPipe.Pipes.Length.Should().Be(3);
			branchPipe.Pipes[0].Should().Be(b);
			branchPipe.Pipes[1].Should().Be(a);
			branchPipe.Pipes[2].Should().Be(c);
		}

		[Fact]
		public void BranchPipe_Enqueues_ToAllInnerPipes()
		{
			var pipes = Enumerable.Repeat<Func<Mock<IMessagePipe<int>>>>(() =>
			{
				var mock = new Mock<IMessagePipe<int>>(MockBehavior.Strict);

				mock.Setup(pipe => pipe.Enqueue(7)).Verifiable();

				return mock;
			}, 10).Select(x => x()).ToArray();

			var branchPipe = new BranchMessagePipe<int>();

			branchPipe.AddRange(pipes.Select(x => x.Object).ToArray());

			branchPipe.Enqueue(7);

			foreach (var pipe in pipes)
			{
				pipe.VerifyAll();
			}
		}
	}
}
