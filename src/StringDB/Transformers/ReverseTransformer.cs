using JetBrains.Annotations;

using System.Runtime.CompilerServices;

namespace StringDB.Transformers
{
	/// <inheritdoc />
	/// <summary>
	/// Reverses the post/pre positions of a transformer.
	/// </summary>
	/// <typeparam name="TPost">Previously the TPre.</typeparam>
	/// <typeparam name="TPre">Previously the TPost.</typeparam>
	[PublicAPI]
	public sealed class ReverseTransformer<TPost, TPre> : ITransformer<TPost, TPre>
	{
		private readonly ITransformer<TPre, TPost> _transformer;

		/// <summary>
		/// Create a reverse transformer.
		/// </summary>
		/// <param name="transformer">The transformer to reverse.</param>
		public ReverseTransformer(ITransformer<TPre, TPost> transformer) => _transformer = transformer;

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPre TransformPre(TPost pre) => _transformer.TransformPost(pre);

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPost TransformPost(TPre post) => _transformer.TransformPre(post);
	}
}