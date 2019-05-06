using JetBrains.Annotations;

namespace StringDB.IO
{
	/// <summary>
	/// The default optimal reading token implementation.
	/// </summary>
	[PublicAPI]
	public sealed class OptimalTokenSource : IOptimalTokenSource
	{
		/// <inheritdoc/>
		public bool OptimalReadingTime { get; set; }

		/// <inheritdoc/>
		public void SetOptimalReadingTime(bool value) => OptimalReadingTime = value;
	}
}