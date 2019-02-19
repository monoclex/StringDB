using System;

namespace StringDB.Tests
{
	public class MockTransformer : ITransformer<int, string>
	{
		public int PreTransform { get; private set; }

		public string Transform(int pre)
		{
			PreTransform = pre;
			return pre.ToString();
		}

		public string PostTransform { get; private set; }

		public int Transform(string post)
		{
			PostTransform = post;
			return Convert.ToInt32(post);
		}
	}
}