using StringDB.Reader;
using StringDB.Writer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StringDB {
	/// <summary>A StringDB Database.</summary>
	public class Database : IEnumerable<ReaderPair>, IDisposable {
		/// <summary>Create a new StringDB database.</summary>
		/// <param name="stream">The stream to read/write to.</param>
		/// <param name="dbm">The DatabaseMode to be in.</param>
		/// <param name="dbv">The version of the database to read/write in</param>
		/// <param name="keepStreamOpen">If the stream should be kept open.<para>Note that in NET 2.0, 3.5, or 4.0, this is not guaranteed to work.</para></param>
		private Database(Stream stream, DatabaseMode dbm = DatabaseMode.ReadWrite, DatabaseVersion dbv = DatabaseVersion.Latest, bool keepStreamOpen = false) {
			this._stream = stream ?? throw new ArgumentNullException("stream");

			if (this.Readable(dbm))
				this._reader = new Reader.StreamReader(this._stream, dbv, keepStreamOpen);
			else this._reader = new InoperableReader(); //by using inoperable readers we prevent reading from ever happening
			
			if (this.Writable(dbm))
				this._writer = new Writer.StreamWriter(this._stream, dbv, keepStreamOpen);
			else this._writer = new InoperableWriter(); //by using inoperable writers we prevent writing from ever happening

			if (this.Writable(dbm)) //if we're trying to write at all
				if (this._stream.Length > 0) //make sure there are indexes to be read
					using (var reader = new Reader.StreamReader(this._stream, dbv, true)) { //we can't trust the reader to be set
						var indexChain = reader.GetReaderChain();

						if (!(this._writer is Writer.StreamWriter))
							throw new Exception("The Writer isn't a stream writer. This is at creator's fault - submit this as an issue to the github repo ( https://www.github.com/SirJosh3917/StringDB )");

						//have to cast - Load is an internal thing
						((Writer.StreamWriter)this._writer).Load(indexChain.IndexChainWrite, indexChain.IndexChain);
					}
		}

		/// <summary>Create a new StringDB database.</summary>
		/// <param name="stream">The stream to read/write to.</param>
		public static Database FromStream(Stream stream) =>
			new Database(stream);

		/// <summary>Create a new StringDB database.</summary>
		/// <param name="stream">The stream to read/write to.</param>
		/// <param name="dbm">The DatabaseMode to be in.</param>
		/// <param name="dbv">The version of the database to read/write in</param>
		/// <param name="keepStreamOpen">If the stream should be kept open.<para>Note that in NET 2.0, 3.5, or 4.0, this is not guaranteed to work.</para></param>
		public static Database FromStream(Stream stream, DatabaseMode dbm = DatabaseMode.ReadWrite,
			DatabaseVersion dbv = DatabaseVersion.Latest, bool keepStreamOpen = false) =>
			new Database(stream, dbm, dbv, keepStreamOpen);

		/// <summary>Create a new StringDB database.</summary>
		/// <param name="stream">The stream to read/write to.</param>
		/// <param name="dbm">The DatabaseMode to be in.</param>
		/// <param name="dbv">The version of the database to read/write in</param>
		/// <param name="keepStreamOpen">If the stream should be kept open.<para>Note that in NET 2.0, 3.5, or 4.0, this is not guaranteed to work.</para></param>
		public static Database FromStream(Stream stream, DatabaseVersion dbv = DatabaseVersion.Latest,
			 DatabaseMode dbm = DatabaseMode.ReadWrite, bool keepStreamOpen = false) =>
			new Database(stream, dbm, dbv, keepStreamOpen);

		/// <summary>Create a new StringDB database.</summary>
		/// <param name="stream">The stream to read/write to.</param>
		/// <param name="dbm">The DatabaseMode to be in.</param>
		/// <param name="dbv">The version of the database to read/write in</param>
		/// <param name="keepStreamOpen">If the stream should be kept open.<para>Note that in NET 2.0, 3.5, or 4.0, this is not guaranteed to work.</para></param>
		public static Database FromStream(Stream stream, bool keepStreamOpen = false,
			DatabaseMode dbm = DatabaseMode.ReadWrite, DatabaseVersion dbv = DatabaseVersion.Latest) =>
			new Database(stream, dbm, dbv, keepStreamOpen);

		/// <summary>Create a new StringDB database.</summary>
		/// <param name="stream">The stream to read/write to.</param>
		/// <param name="dbm">The DatabaseMode to be in.</param>
		/// <param name="dbv">The version of the database to read/write in</param>
		/// <param name="keepStreamOpen">If the stream should be kept open.<para>Note that in NET 2.0, 3.5, or 4.0, this is not guaranteed to work.</para></param>
		public static Database FromStream(Stream stream, bool keepStreamOpen = false,
			 DatabaseVersion dbv = DatabaseVersion.Latest, DatabaseMode dbm = DatabaseMode.ReadWrite) =>
			new Database(stream, dbm, dbv, keepStreamOpen);

		private Stream _stream { get; set; }
		private IReader _reader { get; set; }
		private IWriter _writer { get; set; }

		/// <summary>Will tell the reader to see how much of an overhead StringDB is using.</summary>
		/// <returns></returns>
		public ulong StringDBByteOverhead() => this._reader.GetOverhead();

		/// <summary>Inserts a single piece of data into the database.<para>If you have multiple pieces of data to insert, it's best practice to use InsertRange whenever possible. The more data you can put into a single InsertRange, the less space the database file will take up.</para></summary>
		/// <param name="index">The index to use so you can retrieve the data later.</param>
		/// <param name="data">The data that correlates with the index.</param>
		public void Insert(string index, string data) => this._writer.Insert(index, data);

		/// <summary>Inserts a range of data into the database.</summary>
		/// <param name="range">The pieces of data to insert.</param>
		public void InsertRange(ICollection<KeyValuePair<string, string>> range) => this._writer.InsertRange(range);

		/// <summary>Gets the value of a specific index.</summary>
		/// <param name="index">The index to use when looking for the data.</param>
		/// <returns>A string that holds the data correlating to the index specified.</returns>
		public string GetValueOf(string index) =>
			GetString(this._reader.GetValueOf(index));
		
		/// <summary>Returns all the values of a specific index. Since you're not forced to only have one index correlate with some data using StringDB, you can have the same index correlate to multiple pieces of data.<para>However, you will have to first read over the entire document to find every index and it's position, so it's recommened to not use this, or write multiple pieces of data to the same index.</para></summary>
		/// <param name="index">The index to search for data with</param>
		/// <returns>A string[] that holds every piece of data that correlates with the index specified.</returns>
		public string[] GetValuesOf(string index) =>
			GetStringArray(this._reader.GetValuesOf(index));
		
		/// <summary>Reads every index into memory and outputs it.<para>If you're trying to iterate over this database object, it's recommended to use a foreach loop.</para></summary>
		/// <returns>A string[] that holds every single index.</returns>
		public string[] Indexes() =>
			GetStringArray(this._reader.GetIndexes());
		
		/// <summary>Gets the first index of the StringDB</summary>
		/// <returns>A string, that is the first index.</returns>
		public string FirstIndex() => this._reader.FirstIndex().Index; /// <inheritdoc/>
			
		public IEnumerator<ReaderPair> GetEnumerator() => this._reader.GetEnumerator(); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => this._reader.GetEnumerator();

		//private methods to check for database modes and such

		private bool Readable(DatabaseMode e) =>
			e == DatabaseMode.ReadWrite || e == DatabaseMode.Read;

		private bool Writable(DatabaseMode e) =>
			e == DatabaseMode.ReadWrite || e == DatabaseMode.Write;

		private bool ReadAndWriteable(DatabaseMode e) =>
			Readable(e) && Writable(e);

		//for compatability reasons
		//TODO: put it somewhere else
		internal static string GetString(byte[] bytes) {
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2
			return System.Text.Encoding.UTF8.GetString(
								bytes, 0, bytes.Length
							);
#else
			return System.Text.Encoding.UTF8.GetString(
								bytes
							);
#endif
		}

		//TODO: put it somewhere else
		internal static byte[] GetBytes(string @string) =>
			System.Text.Encoding.UTF8.GetBytes(@string);

		private static string[] GetStringArray(byte[][] bytes) {
			var res = new string[bytes.Length];

			for (var i = 0u; i < bytes.Length; i++)
				res[i] = GetString(bytes[i]);

			return res;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		/// <summary>Dispose this class</summary>
		/// <param name="disposing">Dispose it!!!</param>
		protected virtual void Dispose(bool disposing) {
			if (!this.disposedValue) {
				if (disposing) {
					this._stream.Flush();
					this._stream.Dispose();
					this._reader.Dispose();
					this._writer.Dispose();
				}

				this._stream = null;
				this._reader = null;
				this._writer = null;

				this.disposedValue = true;
			}
		}
		
		/// <summary>Finalizer</summary>
		~Database() {
			Dispose(false);
		}
		
		/// <summary>Dispose </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}

	/// <summary>The mode to read/write a database in. Attempting to do an operation not permitted will throw exceptions.</summary>
	public enum DatabaseMode {
		/// <summary>Allows for both reading and writing.
		/// Attempting to read will succeed.
		/// Attempting to write will succeed.</summary>
		ReadWrite = 1,

		/// <summary>Allows for both reading and writing.
		/// Attempting to read will FAIL.
		/// Attempting to write will succeed.</summary>
		Write = 2,

		/// <summary>Allows for both reading and writing.
		/// Attempting to read will succeed.
		/// Attempting to write will FAIL.</summary>
		Read = 4
	}

	/// <summary>The version of the database. This can't be inferred</summary>
	public enum DatabaseVersion {
		/// <summary>The most current database version.</summary>
		Latest = (int)DatabaseVersion.Verson300,

		/// <summary>The original database structure as of version 1.0.0</summary>
		Version100 = 1,

		//anything that breaks how the database is structured should get a new major version number

		/// <summary>The database structure as of version 2.0.0</summary>
		Version200 = 2,

		/// <summary>The database structure as of version 2.1.0</summary>
		Verson210 = 3,

		/// <summary>The database structure as of version 3.0.0</summary>
		Version300 = 4
	}
}
