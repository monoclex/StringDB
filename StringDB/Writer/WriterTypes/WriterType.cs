using System.IO;

namespace StringDB.Writer {

	internal abstract class WriterType<T> {

		public WriterType() {
		}

		private static StringWriterType _strCache = null;
		public static StringWriterType StringWriterType => _strCache ?? (_strCache = new StringWriterType());

		private static ByteArrayWriterType _byteCache = null;
		public static ByteArrayWriterType ByteArray => _byteCache ?? (_byteCache = new ByteArrayWriterType());

		private static StreamWriterType _streamCache = null;
		public static StreamWriterType StreamWriterType => _streamCache ?? (_streamCache = new StreamWriterType());

		public abstract long Judge(T item);

		public abstract void Write(BinaryWriter bw, T item);
	}

	internal static class WriterTypeManager {
		//thread safe so we'll put some static definitions here

		private static StringWriterType _strCache = null;
		public static StringWriterType String => _strCache ?? (_strCache = new StringWriterType());

		private static ByteArrayWriterType _byteCache = null;
		public static ByteArrayWriterType ByteArray => _byteCache ?? (_byteCache = new ByteArrayWriterType());

		private static StreamWriterType _streamCache = null;
		public static StreamWriterType Stream => _streamCache ?? (_streamCache = new StreamWriterType());
	}
}