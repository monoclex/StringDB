using JetBrains.Annotations;

namespace StringDB.Transformers
{
	/// <summary>
	/// Some handy extensions to make using a reverse transformer easier.
	/// </summary>
	[PublicAPI]
	public static class ReverseTransformerExtensions
	{
		/// <summary>
		/// Reverses a transformer's <typeparamref name="TPre"/> and <typeparamref name="TPost"/> positions.
		/// </summary>
		/// <param name="transformer">The transformer to reverse.</param>
		/// <returns>A reversed transformer.</returns>
		[NotNull]
		public static ReverseTransformer<TPost, TPre> Reverse<TPre, TPost>
		(
			[NotNull] this ITransformer<TPre, TPost> transformer
		)
			=> new ReverseTransformer<TPost, TPre>(transformer);
	}
}