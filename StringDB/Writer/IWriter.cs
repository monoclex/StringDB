using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Writer {
	public interface IWriter {
		void Insert(string index, string data);
		void InsertRange(Dictionary<string, string> data);
		void InsertRange(Tuple<string, string>[] data);
	}
}