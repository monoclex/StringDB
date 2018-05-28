using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StringDB.Tester {
	class Program {
		static void Main(string[] args) {
			using (var fs = File.Open("test.db", FileMode.OpenOrCreate)) {
				var db = new Database(fs, DatabaseMode.Read);

				Stopwatch a = new Stopwatch();
				a.Start();
				Console.WriteLine(db.Get("BABCIEBCGEIIAFHIEEEDCCDCCAIADEEDDGAAIIBBCHEGIHFGADFHAEIHFIAIEAICIFECEHAEDDBDHFCD"));
				a.Stop();

				Console.WriteLine(a.ElapsedMilliseconds + " - " + a.ElapsedTicks);

				//Write(db);
			}
			
			Console.ReadLine();
		}
		
		static void Write(Database db) {
			var r = new Random(1337);

			Console.WriteLine("Generating Data");

			var chrdata = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
			int max = 1000000;

			for(uint i = 0; i < max; i++) {
				var strb_id = new StringBuilder();
				var strb_val = new StringBuilder();

				for (uint j = 0; j < 80; j++)
					strb_id.Append(chrdata[r.Next(0, 9)]);

				for (uint j = 0; j < 1280; j++)
					strb_val.Append(chrdata[r.Next(0, 9)]);

				db.Insert(strb_id.ToString(), strb_val.ToString());
			}
		}
	}
}
