using JetBrains.Annotations;

namespace StringDB.LazyLoaders
{
	/// <summary>
	/// A lazy loader that only returns the value given to it.
	/// </summary>
	[PublicAPI]
	public sealed class ValueLoader<T> : ILazyLoader<T>
	{
		private readonly T _value;

		/// <summary>
		/// Create a new <see cref="ValueLoader{T}"/>.
		/// </summary>
		/// <param name="value">The value to immedietly return upon calling <see cref="Load"/>.</param>
		public ValueLoader(T value) => _value = value;

		/// <inheritdoc />
		public T Load() => _value;
	}
}