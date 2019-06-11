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
		/// The result passed is not accepted. Reading is to be
		/// continued until the end of the database or a completed
		/// state.
		/// </summary>
		NotAccepted,

		/// <summary>
		/// The result passed is accepted. It is scheduled for the
		/// entry to be processed, and reading is to be continued.
		/// </summary>
		Accepted,

		/// <summary>
		/// The query is done executing. The result passed is accepted
		/// and scheduled to be processed, and the query will finish
		/// after processing completes.
		/// </summary>
		Completed
	}
}