using JetBrains.Annotations;

namespace StringDB.LazyLoaders
{
	/// <summary>
	/// A lazy loader that only returns the value given to it.
	/// </summary>
	[PublicAPI]
	public class ValueLoader<T> : ILazyLoader<T>
	{
		private readonly T _value;

		/// <inheritdoc />
		public ValueLoader(T value) => _value = value;

		/// <inheritdoc />
		public T Load() => _value;
	}
}