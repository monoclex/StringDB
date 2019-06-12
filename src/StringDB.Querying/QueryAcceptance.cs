using JetBrains.Annotations;

namespace StringDB.Querying
{
	/// <summary>
	/// Describes the various ways a given query may
	/// accept a result.
	/// </summary>
	[PublicAPI]
	public enum QueryAcceptance
	{
		/// <summary>
		/// The query should continue to be executed.
		/// </summary>
		Continue,

		/// <summary>
		/// The query is done executing. The result passed is accepted
		/// and scheduled to be processed, and the query will finish
		/// after processing completes.
		/// </summary>
		Completed
	}
}