namespace StringDB.Transformers
{
	public sealed class ReverseTransformer<TPost, TPre> : ITransformer<TPost, TPre>
	{
		private readonly ITransformer<TPre, TPost> _transformer;

		public ReverseTransformer(ITransformer<TPre, TPost> transformer) => _transformer = transformer;

		public TPre TransformPre(TPost pre) => _transformer.TransformPost(pre);

		public TPost TransformPost(TPre post) => _transformer.TransformPre(post);
	}

	public static class ReverseTransformerExtensions
	{
		public static ReverseTransformer<TPost, TPre> Reverse<TPre, TPost>(this ITransformer<TPre, TPost> transformer)
			=> new ReverseTransformer<TPost, TPre>(transformer);
	}
}