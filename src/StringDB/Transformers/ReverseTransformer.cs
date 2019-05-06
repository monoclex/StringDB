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
		/// <summary>
		/// The transformer being used under the hood.
		/// </summary>
		[NotNull]
		public ITransformer<TPre, TPost> Transformer { get; }

		/// <summary>
		/// Create a reverse transformer.
		/// </summary>
		/// <param name="transformer">The transformer to reverse.</param>
		public ReverseTransformer(ITransformer<TPre, TPost> transformer) => Transformer = transformer;

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPre TransformPre(TPost pre) => Transformer.TransformPost(pre);

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPost TransformPost(TPre post) => Transformer.TransformPre(post);
	}
}