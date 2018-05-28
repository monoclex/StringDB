using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB.Tester {
	class Program {
		static void Main(string[] args) {
			using (var fs = File.Open("example.db", FileMode.OpenOrCreate)) {
				var db = new Database(fs);
				
					foreach (var i in db.Indexes())
						Console.WriteLine(i);

				Write(db);
			}

			Console.ReadLine();
		}
		
		static void Write(Database db) {
			var r = new Random(1337);

			Console.WriteLine("Generating Data");

			var chrdata = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
			int max = 100000;
			var data = new Dictionary<string, string>(max);
			
			for(uint i = 0; i < max; i++) {
				var strb_id = new StringBuilder();
				var strb_val = new StringBuilder();

				for (uint j = 0; j < 80; j++)
					strb_id.Append(chrdata[r.Next(0, 9)]);

				for (uint j = 0; j < 1280; j++)
					strb_val.Append(chrdata[r.Next(0, 9)]);

				try {
					data.Add(strb_id.ToString(), strb_val.ToString());
				} catch (Exception ini) {  }
			}

			for(uint i = 0; i < 5; i++)
				db.InsertRange(data);
		}
	}
}
