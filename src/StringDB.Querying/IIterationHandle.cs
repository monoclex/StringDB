using JetBrains.Annotations;

using System;

namespace StringDB.Querying
{
	/// <summary>
	/// A handle that an <see cref="IIterationManager{TKey, TValue}"/> gives
	/// for iteration. Use it to keep track of iteration progress and such.
	/// </summary>
	[PublicAPI]
	public interface IIterationHandle : IDisposable
	{
		/// <summary>
		/// This is the task running in the background for iteration.
		/// </summary>
		[NotNull]
		BackgroundTask IterationTask { get; }

		/// <summary>
		/// The current index the iteration is at.
		/// </summary>
		int Index { get; }

		/// <summary>
		/// If the iteration is completed, or, at the end.
		/// </summary>
		bool AtEnd { get; }
	}
}