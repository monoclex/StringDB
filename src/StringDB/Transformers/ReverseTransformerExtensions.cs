namespace StringDB.Transformers
{
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