using JetBrains.Annotations;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A simple encapsulation of background tasks.
	/// </summary>
	[PublicAPI]
	public sealed class BackgroundTask : IDisposable
	{
		/// <summary>
		/// The cancellation token source used in the <see cref="Task"/>.
		/// </summary>
		[NotNull] public CancellationTokenSource CancellationTokenSource { get; }

		/// <summary>
		/// The <see cref="System.Threading.Tasks.Task"/> running.
		/// </summary>
		[NotNull] public Task Task { get; }

		/// <summary>
		/// Starts up the Task
		/// </summary>
		/// <param name="code"></param>
		public BackgroundTask
		(
			[NotNull] Func<CancellationToken, Task> code
		)
		{
			CancellationTokenSource = new CancellationTokenSource();
			Task = code(CancellationTokenSource.Token);
		}

		public void Dispose() => CancellationTokenSource.Cancel();
	}

	/// <summary>
	/// A simple encapsulation of background tasks.
	/// </summary>
	[PublicAPI]
	public sealed class BackgroundTask<TResult> : IDisposable
	{
		/// <summary>
		/// The cancellation token source used in the <see cref="Task"/>.
		/// </summary>
		[NotNull] public CancellationTokenSource CancellationTokenSource { get; }

		/// <summary>
		/// The <see cref="System.Threading.Tasks.Task{TResult}"/> running.
		/// </summary>
		[NotNull] public Task<TResult> Task { get; }

		/// <summary>
		/// Starts up the Task
		/// </summary>
		/// <param name="code"></param>
		public BackgroundTask
		(
			[NotNull] Func<CancellationToken, Task<TResult>> code
		)
		{
			CancellationTokenSource = new CancellationTokenSource();
			Task = code(CancellationTokenSource.Token);
		}

		public void Dispose() => CancellationTokenSource.Cancel();
	}
}