using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Exceptions {
	/// <summary>An exception that gets thrown when attempting to get a Type that doesn't exist.</summary>
	public class TypeHandlerDoesntExist : Exception {
		internal TypeHandlerDoesntExist(object t)
			: base($"The TypeHandler ({t}) doesn't exist. See RegisterType if you'd like to add a type, or if you're trying to read from this database, then there are missing TypeHandlers.") { }
	}
}