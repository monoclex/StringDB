using JetBrains.Annotations;

namespace StringDB.IO
{
	/// <summary>
	/// What the next item could be, after peeking.
	/// </summary>
	[PublicAPI]
	public enum NextItemPeek
	{
		Index,
		Jump,
		EOF
	}
}