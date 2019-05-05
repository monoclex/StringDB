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
		/// <inheritdoc/>
		public T TransformPost(T post) => post;

		/// <inheritdoc/>
		public T TransformPre(T pre) => pre;
	}
}