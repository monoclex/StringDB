using System;
using System.Collections.Generic;

namespace StringDB.Writer {
	/// <summary>A writer. It writes additional pieces of data to the stream.</summary>
	public interface IWriter : IDisposable {
		/// <summary>Inserts a single piece of data into the stream.<para>If you have multiple pieces of data to insert, it's best practice to use InsertRange whenever possible. The more data you can put into a single InsertRange, the less space the database file will take up.</para></summary>
		/// <param name="index">The index to use so you can retrieve the data later.</param>
		/// <param name="data">The data that correlates with the index.</param>
		void Insert(string index, string data);

		/// <summary>Inserts a range of data into the database.</summary>
		/// <param name="data">The pieces of data to insert.</param>
		void InsertRange(ICollection<KeyValuePair<string, string>> data);
	}
}