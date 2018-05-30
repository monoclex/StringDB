using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StringDB.Tester {
	class Program {
		static void Main(string[] args) {
			using (var fs = File.Open("teaaastss.db", FileMode.OpenOrCreate)) {
				var db = new Database(fs, DatabaseMode.ReadWrite);
				Write(db);

				var a = new Stopwatch();
				a.Start();

				var ind = db.IndexAfter(db.FirstIndex());

				while(true) {
					Console.WriteLine(db.lastAsk);
					ind = db.IndexAfter(ind);
	
				}

				Console.ReadLine();

				//var firstIndex = db.FirstIndex();
				//var index = firstIndex;

				while(true) {
					foreach(var i in db.Indexes())
						Console.WriteLine(i);
					Console.ReadKey(true);
					//index = db.IndexAfter(index);
				}

				Console.WriteLine(a.ElapsedMilliseconds + " - " + a.ElapsedTicks);
			}
			
			Console.ReadLine();
		}
		
		static void Write(Database db) {
			var r = new Random(1337);

			Console.WriteLine("Generating Data");

			var chrdata = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
			int max = 10;
		
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
