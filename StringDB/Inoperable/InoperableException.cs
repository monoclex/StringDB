using System;

namespace StringDB.Inoperable {
	/// <summary>This exception gets thrown whenever you make a call to an inoperable class.</summary>
	public class InoperableException : Exception, IInoperable {
		/// <summary>The message of this exception every time.</summary>
		public const string InoperableString = "This feature is not available to be operated. If you are using a database, consider changing the Database Mode.";

		/// <summary>Create one. /shrug</summary>
		public InoperableException() : base(InoperableString) { }
	}
}