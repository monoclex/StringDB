using System.Text;

namespace StringDB.Transformers
{
	/// <inheritdoc />
	/// <summary>
	/// Transforms a <see cref="T:System.Byte" /> into a <see cref="T:System.String" />.
	/// </summary>
	public sealed class StringTransformer : ITransformer<byte[], string>
	{
		/// <inheritdoc />
		public string TransformPre(byte[] pre) => Encoding.UTF8.GetString(pre);

		/// <inheritdoc />
		public byte[] TransformPost(string post) => Encoding.UTF8.GetBytes(post);
	}
}