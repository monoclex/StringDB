using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB {
	internal static class Consts {

		/// <summary>Used for seperating indexes from data. This is why you can't have indexes with lengths more then 253.</summary>
		public const byte IndexSeperator = 0xFF;
	}
}
