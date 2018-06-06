using StringDB.Inoperable;
using System;
using System.Collections.Generic;

namespace StringDB.Writer {
	/// <summary>Inoperable class. Any method called will throw an InoperableException.</summary>
	public class InoperableWriter : IWriter, IInoperable {
		/// <summary>Throws an InoperableException</summary><returns>Throws an InoperableException</returns>
		public void Insert(string index, string data) => throw new InoperableException();

		/// <summary>Throws an InoperableException</summary><returns>Throws an InoperableException</returns>
		public void InsertRange(ICollection<KeyValuePair<string, string>> data) => throw new InoperableException();

		/// <summary>Throws an InoperableException</summary><returns>Throws an InoperableException</returns>
		public void InsertRange(params ICollection<KeyValuePair<string, string>>[] data) => throw new InoperableException();
	}
}