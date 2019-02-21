using System.Text;

namespace StringDB.Transformers
{
	/// <summary>
	/// Transforms a <see cref="byte[]"/> into a <see cref="string"/>.
	/// </summary>
	public sealed class StringTransformer : ITransformer<byte[], string>
	{
		public string TransformPre(byte[] pre) => Encoding.UTF8.GetString(pre);

		public byte[] TransformPost(string post) => Encoding.UTF8.GetBytes(post);
	}
}