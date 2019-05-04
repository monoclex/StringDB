using JetBrains.Annotations;

using System;

namespace StringDB.LazyLoaders
{
	/// <summary>
	/// Caches the result of an <see cref="ILazyLoader{T}"/>.
	/// </summary>
	[PublicAPI]
	public sealed class CachedLoader<T> : ILazyLoader<T>, IDisposable
	{
		private readonly ILazyLoader<T> _inner;

		private bool _loaded;
		private T _value;

		public CachedLoader([NotNull] ILazyLoader<T> inner)
			=> _inner = inner;

		public T Load()
		{
			if (!_loaded)
			{
				_value = _inner.Load();
				_loaded = true;
			}

			return _value;
		}

		public void Dispose()
		{
			if (_value is IDisposable disposable)
			{
				disposable.Dispose();
			}

			_value = default;
		}
	}
}