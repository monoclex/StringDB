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

		/// <summary>
		/// Create a new <see cref="ThreadLockLoader{T}"/>.
		/// </summary>
		/// <param name="inner">The lazy loader to wrap in a lock when calling.</param>
		/// <param name="lock">The object to lock on.</param>
		public ThreadLockLoader
		(
			[NotNull] ILazyLoader<T> inner,
			[NotNull] object @lock
		)
		{
			_lock = @lock;
			_inner = inner;
		}

		/// <inheritdoc />
		public T Load()
		{
			lock (_lock)
			{
				return _inner.Load();
			}
		}
	}
}