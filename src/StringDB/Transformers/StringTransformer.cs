using System.Text;

namespace StringDB.Transformers
{
	public sealed class StringTransformer : ITransformer<byte[], string>
	{
		public string Transform(byte[] pre) => Encoding.UTF8.GetString(pre);

		public byte[] Transform(string post) => Encoding.UTF8.GetBytes(post);
	}
}