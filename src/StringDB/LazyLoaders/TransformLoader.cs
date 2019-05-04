using JetBrains.Annotations;

using StringDB.Databases;

namespace StringDB.LazyLoaders
{
	/// <summary>
	/// The <see cref="ILazyLoader{T}"/> used in a <see cref="TransformDatabase{TPreKey, TPreValue, TPostKey, TPostValue}"/>
	/// </summary>
	/// <typeparam name="TPre">The type before transformation.</typeparam>
	/// <typeparam name="TPost">The type after transformation.</typeparam>
	[PublicAPI]
	public sealed class TransformLazyLoader<TPre, TPost> : ILazyLoader<TPost>
	{
		private readonly ITransformer<TPre, TPost> _transformer;
		private readonly ILazyLoader<TPre> _pre;

		/// <summary>
		/// Creates a new <see cref="TransformLazyLoader{TPre, TPost}"/>.
		/// </summary>
		/// <param name="pre">The lazy loader with the value.</param>
		/// <param name="transformer">The transformer to transform the value.</param>
		public TransformLazyLoader
		(
			[NotNull] ILazyLoader<TPre> pre,
			[NotNull] ITransformer<TPre, TPost> transformer
		)
		{
			_pre = pre;
			_transformer = transformer;
		}

		/// <inheritdoc />
		public TPost Load()
		{
			var loaded = _pre.Load();

			return _transformer.TransformPre(loaded);
		}
	}
}