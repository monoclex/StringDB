using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StringDB
{
	// note: no [PublicAPI]
	/// <summary>
	/// Used to allow an <see cref="ILazyLoader{T}"/> to be awaited.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public struct LazyLoaderAwaiter<T> : INotifyCompletion
	{
		public ILazyLoader<T> LazyLoader;
		private T _result;
		public bool IsCompleted { get; private set; }

		public T GetResult()
		{
			if (!IsCompleted)
			{
				_result = LazyLoader.Load();
				IsCompleted = true;
			}

			return _result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnCompleted(Action continuation)
			=> new Task(continuation).Start();
	}
}