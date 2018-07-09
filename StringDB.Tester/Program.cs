using Newtonsoft.Json;
using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace StringDB.Tester {

	internal class Program {
		private static KeyValuePair<byte[], byte[]> CacheKVP = new KeyValuePair<byte[], byte[]>(new byte[100], new byte[1000]);

		public static IEnumerable<KeyValuePair<byte[], byte[]>> GetSampleData() {
			for (var i = 0; i < 1_000_000; i++)
				yield return CacheKVP;
		}

		private static void Main() {

			using (var newdb = Database.FromFile("example.db")) {
				newdb.InsertRange(new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("INDEX_1", "VALUE_FOR_INDEX_1"),
					new KeyValuePair<string, string>("INDEX_2", "VALUE_FOR_INDEX_2"),
				});
				newdb.InsertRange(new List<KeyValuePair<string, string>>() {
					new KeyValuePair<string, string>("INDEX_3", "VALUE_FOR_INDEX_3"),
					new KeyValuePair<string, string>("INDEX_4", "VALUE_FOR_INDEX_4"),
				});

				newdb.OverwriteValue(newdb.Get("INDEX_3"), "new value for the index at pos 3");

				Console.WriteLine(newdb.Get("INDEX_3").GetValueAs<string>());

				foreach (var i in newdb)
					Console.WriteLine(i);
			}

			Console.ReadLine();

			using (var db = Database.FromFile("struc2t.db")) {

				db.Insert("BLUE", "IM");
				db.OverwriteValue(db.Get("BLUE"), "I'M");
				Console.WriteLine(db.Get("BLUE").GetValueAs<string>());

				Console.ReadLine();

				//var len = 0;
				//foreach (var i in db) len++;

				//if(len == 0)
				//	db.InsertRange(GetSampleData());

				db.Insert("im", "gay");
				var val = db.Get("im");
				Console.WriteLine(val.GetValueAs<string>());

			}

			Console.ReadLine();
		}

		private static void Time(int estTime, Action before, Action method, Action after) {

			var est = GetStopwatch(1, before, method, after);
			var amt = 5_000 / (double)est.ElapsedMilliseconds;

			if (amt < 1)
				amt = 1;

			Console.WriteLine($"Beginning iteration amount: {amt}");

			var time = GetStopwatch((int)amt,
				before, method, (() => { Console.Write('.'); after?.Invoke(); })
			);

			Console.WriteLine($"Took {time.ElapsedMilliseconds} for {amt} operations ({(double)time.ElapsedMilliseconds / amt} est ms/op)");

			//we want it to take estTime seconds

			var timeTaken = (double)estTime / ((double)time.ElapsedMilliseconds / amt);

			if (timeTaken < 1)
				timeTaken = 1;

			Console.WriteLine($"Repeating test {(int)timeTaken} times");

			var elapse = GetStopwatch((int)timeTaken,
				before, method, after
			);

			Console.WriteLine($"Iterations: {(int)timeTaken}\tTotal Elapsed MS: {elapse.ElapsedMilliseconds}\tPer Op: {(double)elapse.ElapsedMilliseconds / (double)(int)timeTaken}");
		}

		private static Stopwatch GetStopwatch(int times, Action before, Action method, Action after) {
			var stp = new Stopwatch();

			for (var i = 0; i < times; i++) {
				before?.Invoke();

				if (method == null) throw new NullReferenceException(nameof(method));

				stp.Start();
#pragma warning disable CC0031 // Check for null before calling a delegate
				method();
#pragma warning restore CC0031 // Check for null before calling a delegate
				stp.Stop();

				after?.Invoke();
			}

			return stp;
		}
	}
}