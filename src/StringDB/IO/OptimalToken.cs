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

	/// <summary>
	/// The default optimal reading token implementation.
	/// </summary>
	[PublicAPI]
	public sealed class OptimalToken : IOptimalTokenSource
	{
		/// <inheritdoc/>
		public bool OptimalReadingTime { get; set; }

		/// <inheritdoc/>
		public void SetOptimalReadingTime(bool value) => OptimalReadingTime = value;
	}
}