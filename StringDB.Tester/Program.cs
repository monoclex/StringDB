using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StringDB.Tester
{
    class Program
    {
		static void Main(string[] args) {
			Insertations();
			
			using (var db = Database.FromFile("game on your phone.db")) {
				db.InsertRange(GetValues(100_000, "example test value"));

				foreach (var i in db)
					Console.WriteLine(i.ToString());
			}

			Console.ReadLine();
		}

		static void Insertations() {
			foreach (var i in GetValues(100, "ree"))
				Console.WriteLine(i.ToString());
		}
		
		static IEnumerable<KeyValuePair<string, string>> GetValues(int amount, string dataValue) {
			for(int iteration = 0; iteration < amount; iteration++)
				yield return new KeyValuePair<string, string>(iteration.ToString(), dataValue);
		}

		static void OverwriteExample() {
			using (var db = Database.FromFile("Overwrite.db")) {

				var itm = db.GetByIndex("Hello"); //try to find "Hello"

				if (itm == null) { //if it doesn't exist, create it
					db.Insert("Hello", "World");
					itm = db.GetByIndex("Hello"); //now get "Hello"
				}

				Console.WriteLine(itm.ToString()); //write the current value of it

				db.OverwriteValue(itm, "Continent"); //change it to Continent

				itm = db.GetByIndex("Hello"); //re-get it by the index ( though itm.Value also changes so you can reuse the ReaderPair )

				Console.WriteLine(itm.ToString()); //prove that it's changed

				Console.WriteLine("done");
			}
		}

		private static void Benchmark(Action act, int iterations) {
			GC.Collect();
			act.Invoke(); // run once outside of loop to avoid initialization costs
			Stopwatch sw = Stopwatch.StartNew();
			for (int i = 1; i <= iterations; i++) {
				act.Invoke();
			}
			sw.Stop();
			Console.WriteLine($"{((decimal)sw.ElapsedMilliseconds / (decimal)iterations)}ms {((decimal)sw.ElapsedTicks / (decimal)iterations)}t");
		}
	}
}
