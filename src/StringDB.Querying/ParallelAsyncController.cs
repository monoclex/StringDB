using JetBrains.Annotations;

using System.Threading;

namespace StringDB.Querying
{
	/// <summary>
	/// A controller to help control the enumeration and result of a parallel foreach async.
	/// </summary>
	/// <typeparam name="TResult">The type of result.</typeparam>
	[PublicAPI]
	public class ParallelAsyncController<TResult>
	{
		private readonly CancellationTokenSource _cancellationTokenSource;

		/// <summary>
		/// Creates a new <see cref="ParallelAsyncController{TResult}"/>.
		/// </summary>
		/// <param name="cancellationTokenSource">The cancellation token source to use.</param>
		public ParallelAsyncController([NotNull] CancellationTokenSource cancellationTokenSource)
		{
			_cancellationTokenSource = cancellationTokenSource;
			Result = default;
		}

		[NotNull]
		public TResult Result { get; private set; }

		/// <summary>
		/// Gives a result to the controller.
		/// </summary>
		/// <param name="result">The type of result.</param>
		public void ProvideResult([NotNull] TResult result) => Result = result;

		/// <summary>
		/// Cancels the enumeration of the parallel code.
		/// </summary>
		public void Stop() => _cancellationTokenSource.Cancel();
	}
}