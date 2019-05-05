using JetBrains.Annotations;

namespace StringDB.Transformers
{
	/// <inheritdoc/>
	/// <summary>
	/// Applies absolutely no transformation to the objects given to it.
	/// </summary>
	/// <typeparam name="T">The type of object to do nothing to.</typeparam>
	[PublicAPI]
	public sealed class NoneTransformer<T> : ITransformer<T, T>
	{
		/// <summary>
		/// A default, global instance of this <see cref="NoneTransformer{T}"/>.
		/// </summary>
		public static NoneTransformer<T> Default { get; } = new NoneTransformer<T>();

		/// <inheritdoc/>
		public T TransformPost(T post) => post;

		/// <inheritdoc/>
		public T TransformPre(T pre) => pre;
	}
}