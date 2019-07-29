using StringDB.IO;

using System.IO;

namespace StringDB.Fluency
{
	public struct IODatabaseOptions
	{
		public IODatabaseOptions New()
		{
			return new IODatabaseOptions
			{
				Version = StringDBVersion.Latest
			};
		}

		public StringDBVersion Version;

		/// <summary>
		/// Used if <see cref="FileName"/> is null.
		/// </summary>
		public Stream Stream;

		/// <summary>
		/// <see cref="Stream"/> is ignored if this is set to a non-null
		/// value.
		/// </summary>
		public string FileName;

		/// <summary>
		/// Setting this to <c>true</c> makes the reader internally use an
		/// instance of <see cref="ByteBuffer"/> to prevent re-allocation of
		/// byte arrays when reading the index. This has a slight side effect
		/// of if the consumer stores the byte array read from the index for
		/// future use, it may be overwritten with the index of a different
		/// element with the same length. However, this can easily be ignored
		/// if applying a database transformation that consumes the byte array
		/// and doesn't store the result, eg. using StringDB.Ceras to transform
		/// keys and values to classes. Try to set this to true in scenarios
		/// where applicable.
		/// </summary>
		public bool UseByteBuffer;

		public bool LeaveStreamOpen;
	}
}