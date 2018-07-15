using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Exceptions {
	/// <summary>An exception that gets thrown when attempting to register a Type if it already exists</summary>
	public class TypeHandlerExists : Exception {
		internal TypeHandlerExists(Type t)
			: base($"The TypeHandler already exists ({t}). See OverridingRegisterType if you'd like to override existing types.") { }
	}
}