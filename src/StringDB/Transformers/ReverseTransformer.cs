namespace StringDB.Transformers
{
	/// <inheritdoc />
	/// <summary>
	/// Reverses the post/pre positions of a transformer.
	/// </summary>
	/// <typeparam name="TPost">Previously the TPre.</typeparam>
	/// <typeparam name="TPre">Previously the TPost.</typeparam>
	public sealed class ReverseTransformer<TPost, TPre> : ITransformer<TPost, TPre>
	{
		private readonly ITransformer<TPre, TPost> _transformer;

		/// <summary>
		/// Create a reverse transformer.
		/// </summary>
		/// <param name="transformer">The transformer to reverse.</param>
		public ReverseTransformer(ITransformer<TPre, TPost> transformer) => _transformer = transformer;

		/// <inheritdoc />
		public TPre TransformPre(TPost pre) => _transformer.TransformPost(pre);

		/// <inheritdoc />
		public TPost TransformPost(TPre post) => _transformer.TransformPre(post);
	}

	/// <summary>
	/// Some handy extensions to make using a reverse transformer easier.
	/// </summary>
	public static class ReverseTransformerExtensions
	{
		/// <summary>
		/// Reverses a transformer's <typeparamref name="TPre"/> and <typeparamref name="TPost"/> positions.
		/// </summary>
		/// <param name="transformer">The transformer to reverse.</param>
		/// <returns>A reversed transformer.</returns>
		public static ReverseTransformer<TPost, TPre> Reverse<TPre, TPost>(this ITransformer<TPre, TPost> transformer)
			=> new ReverseTransformer<TPost, TPre>(transformer);
	}
}