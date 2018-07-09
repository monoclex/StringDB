using Newtonsoft.Json;
using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringDB.Tester {

	internal class Program {
		private static KeyValuePair<byte[], byte[]> CacheKVP = new KeyValuePair<byte[], byte[]>(new byte[100], new byte[1000]);

		public static IEnumerable<KeyValuePair<byte[], byte[]>> GetSampleData() {
			for (var i = 0; i < 1_000_000; i++)
				yield return CacheKVP;
		}

		private static void Main() {

			using (var db = Database.FromFile("struct.db")) {

				var len = 0;
				foreach (var i in db) len++;

				if(len == 0)
					db.InsertRange(GetSampleData());

				Time(5_000, () => { }, () => { foreach (var i in db) { } }, () => { });
				Time(5_000, () => { }, () => { foreach (var i in db) { i.GetValueAs<byte[]>(); } }, () => { });
				Time(5_000, () => { }, () => { Parallel.ForEach(db, (i) => { }); }, () => { });
				Time(5_000, () => { }, () => { Parallel.ForEach(db, (i) => { lock (db) { } }); }, () => { });
				Time(5_000, () => { }, () => { Parallel.ForEach(db, (i) => { lock (db) { i.GetValueAs<byte[]>(); } }); }, () => { });

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