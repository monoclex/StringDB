namespace StringDB {
	internal static class Consts {

		/// <summary>The maximum value for an index. After this point, bytes are </summary>
		public const int MaxLength = 0xFE;

		/// <summary>Used to tell that this value is deleted.</summary>
		public const byte DeletedValue = 0xFE;

		/// <summary>Used for seperating indexes from data. This is why you can't have indexes with lengths more then 253.</summary>
		public const byte IndexSeperator = 0xFF;

		/// <summary>Used to tell if the next value is a byte</summary>
		public const byte IsByteValue = 0x01;

		/// <summary>Used to tell if the next value is a ushort</summary>
		public const byte IsUShortValue = 0x02;

		/// <summary>Used to tell if the next value is a uint</summary>
		public const byte IsUIntValue = 0x03;

		/// <summary>Used to tell if the next value is a ulong</summary>
		public const byte IsULongValue = 0x04;

		/// <summary>An index with zero length tells it that there's nothing after this.</summary>
		public const byte NoIndex = 0x00;
	}
}
