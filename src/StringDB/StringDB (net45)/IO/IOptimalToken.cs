using JetBrains.Annotations;

namespace StringDB.IO
{
	/// <summary>
	/// Represents the optimal time to read values from a file.
	/// </summary>
	[PublicAPI]
	public interface IOptimalToken
	{
		/// <summary>
		/// If it is the optimal reading time.
		/// </summary>
		bool OptimalReadingTime { get; }
	}
}