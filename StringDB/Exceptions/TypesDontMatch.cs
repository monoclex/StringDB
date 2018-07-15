using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Exceptions {
	public class TypesDontMatch : Exception {
		public TypesDontMatch(Type a, Type b)
			: base($"The data you are trying to read isn't of type ({a}), it is of type ({b})") { }
	}
}