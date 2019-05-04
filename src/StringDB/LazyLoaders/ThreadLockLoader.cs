using JetBrains.Annotations;

using StringDB.Databases;

namespace StringDB.LazyLoaders
{
	/// <summary>
	/// The <see cref="ILazyLoader{T}"/> used in a <see cref="TransformDatabase{TPreKey, TPreValue, TPostKey, TPostValue}"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[PublicAPI]
	public sealed class ThreadLockLoader<T> : ILazyLoader<T>
	{
		private readonly object _lock;
		private readonly ILazyLoader<T> _inner;

		public ThreadLockLoader([NotNull] object @lock, [NotNull] ILazyLoader<T> inner)
		{
			_lock = @lock;
			_inner = inner;
		}

		public T Load()
		{
			lock (_lock)
			{
				return _inner.Load();
			}
		}
	}
}