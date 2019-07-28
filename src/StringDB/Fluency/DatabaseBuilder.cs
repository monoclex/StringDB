using JetBrains.Annotations;

namespace StringDB.Fluency
{
	/// <summary>
	/// A class that allows the extensive usage of extensions to create databases.
	/// </summary>
	[PublicAPI]
	public struct DatabaseBuilder
	{
		public static DatabaseBuilder Instance { get; }
	}
}