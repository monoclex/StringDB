using JetBrains.Annotations;

namespace StringDB.IO
{
	/// <summary>
	/// Controls the value for an <see cref="IOptimalToken"/>
	/// </summary>
	[PublicAPI]
	public interface IOptimalTokenSource : IOptimalToken
	{
		/// <summary>
		/// Sets the value for the optimal reading time.
		/// </summary>
		void SetOptimalReadingTime(bool value);
	}
}