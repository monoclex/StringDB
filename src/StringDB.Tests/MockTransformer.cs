using System;

namespace StringDB.Tests
{
	public class MockTransformer : ITransformer<int, string>
	{
		public int PreTransform { get; private set; }

		public string TransformPre(int pre)
		{
			PreTransform = pre;
			return Convert.ToChar(pre).ToString();
		}

		public string PostTransform { get; private set; }

		public int TransformPost(string post)
		{
			PostTransform = post;
			return Convert.ToInt32(Convert.ToChar(post));
		}
	}
}