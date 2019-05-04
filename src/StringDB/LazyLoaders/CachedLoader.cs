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

		/// <summary>
		/// Creates a new <see cref="CachedLoader{T}"/>.
		/// </summary>
		/// <param name="inner">The inner lazy loader to cache the result of.</param>
		public CachedLoader([NotNull] ILazyLoader<T> inner)
			=> _inner = inner;

		/// <inheritdoc />
		public T Load()
		{
			if (!_loaded)
			{
				_value = _inner.Load();
				_loaded = true;
			}

			return _value;
		}

		/// <inheritdoc />
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