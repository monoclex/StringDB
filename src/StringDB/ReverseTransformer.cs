namespace StringDB
{
	public sealed class ReverseTransformer<TPost, TPre> : ITransformer<TPost, TPre>
	{
		private readonly ITransformer<TPre, TPost> _transformer;

		public ReverseTransformer(ITransformer<TPre, TPost> transformer) => _transformer = transformer;

		public TPre Transform(TPost pre) => _transformer.Transform(pre);

		public TPost Transform(TPre post) => _transformer.Transform(post);
	}
}