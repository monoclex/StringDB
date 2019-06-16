namespace StringDB.Tests.Mocks
{
	public class IntToStringTransformer : ITransformer<int, string>
	{
		public static IntToStringTransformer Default { get; } = new IntToStringTransformer();

		public int TransformPost(string post) => int.Parse(post);

		public string TransformPre(int pre) => pre.ToString();
	}
}