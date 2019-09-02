using JetBrains.Annotations;

using System;

namespace StringDB.Querying
{
	public sealed class IterationHandle : IIterationHandle
	{
		[NotNull] private readonly Func<int> _getIndex;
		[NotNull] private readonly Func<bool> _atEnd;

		public IterationHandle
		(
			[NotNull] BackgroundTask iterationTask,
			[NotNull] Func<int> getIndex,
			[NotNull] Func<bool> atEnd
		)
		{
			IterationTask = iterationTask;
			_getIndex = getIndex;
			_atEnd = atEnd;
		}

		[NotNull]
		public BackgroundTask IterationTask { get; }

		public int Index => _getIndex();

		public bool AtEnd => _atEnd();

		public void Dispose() => IterationTask.Dispose();
	}
}