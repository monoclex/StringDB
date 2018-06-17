using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StringDB.Tester
{
    class Program
    {
		static void Main(string[] args) {
			Console.ReadKey(true);
			using(var db = Database.FromFile("re.db")) {

				Benchmark(() => { var E = db.First(); }, 1000);

				Console.WriteLine("started");
				Benchmark(() => { var E = db.First().Value; }, 1_000_000_000);

				Console.WriteLine("done");
			}

			Console.ReadLine();
		}

		static void New(Stopwatch z) {
			z.Stop();

			Console.WriteLine($"{z.ElapsedMilliseconds}ms, {z.ElapsedTicks}t");

			z.Restart();
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
