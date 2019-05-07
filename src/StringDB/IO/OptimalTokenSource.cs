using JetBrains.Annotations;

namespace StringDB.IO
{
	/// <summary>
	/// The default optimal reading token implementation.
	/// </summary>
	[PublicAPI]
	public sealed class OptimalTokenSource : IOptimalTokenSource, IOptimalToken
	{
		/// <inheritdoc/>
		public bool OptimalReadingTime { get; set; }

		/// <inheritdoc/>
		public IOptimalToken OptimalToken => this;

		/// <inheritdoc/>
		public void SetOptimalReadingTime(bool value) => OptimalReadingTime = value;
	}
}