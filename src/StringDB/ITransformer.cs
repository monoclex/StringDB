using JetBrains.Annotations;

namespace StringDB
{
	/// <summary>
	/// Transforms an item.
	/// </summary>
	/// <typeparam name="TPre">The type of item before the transform.</typeparam>
	/// <typeparam name="TPost">The type of item after the transform.</typeparam>
	[PublicAPI]
	public interface ITransformer<TPre, TPost>
	{
		/// <summary>
		/// Transforms a <typeparamref name="TPre"/> into a <typeparamref name="TPost"/>.
		/// </summary>
		/// <param name="pre">The <typeparamref name="TPre"/> to transform.</param>
		/// <returns>A <typeparamref name="TPost"/>.</returns>
		[NotNull] TPost TransformPre([NotNull] TPre pre);

		/// <summary>
		/// Transforms a <typeparamref name="TPost"/> into a <typeparamref name="TPre"/>
		/// </summary>
		/// <param name="post">The <typeparamref name="TPost"/> to transform.</param>
		/// <returns>A <typeparamref name="TPre"/>.</returns>
		[NotNull] TPre TransformPost([NotNull] TPost post);
	}
}