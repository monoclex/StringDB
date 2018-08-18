using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace StringDB.Tester {

	[TypeManager.AutoRegister]
	public class Example : TypeHandler<int> {
		public override byte Id => 0x2F;

		public override bool Compare(int item1, int item2) => item1 == item2;

		public override long GetLength(int item) => sizeof(int);

		public override int Read(BinaryReader br, long len) => len == 4 ? br.ReadInt32() : throw new Exception("that's not a freaking int");

		public override void Write(BinaryWriter bw, int item) => bw.Write(item);
	}

	internal class Program {
		private static readonly KeyValuePair<byte[], byte[]> CacheKVP = new KeyValuePair<byte[], byte[]>(new byte[100], new byte[1000]);

		public static IEnumerable<KeyValuePair<byte[], byte[]>> GetSampleData() {
			for (var i = 0; i < 1_000_000; i++)
				yield return CacheKVP;
		}

		private static long LongRandom(long min, long max, Random rand) {
			byte[] buf = new byte[8];
			rand.NextBytes(buf);
			long longRand = BitConverter.ToInt64(buf, 0);

			return (Math.Abs(longRand % (max - min)) + min);
		}

		private static void Main() {
			Console.ReadLine();

			Console.WriteLine("S");
			var data = new long[880000 * 2];

			var rng = new Random();

			for (int i = 0; i < data.Length; i++)
				data[i] = LongRandom(long.MinValue, long.MaxValue, rng);

			Console.WriteLine(Marshal.SizeOf(data));

			Console.ReadLine();

			/*
			var ms = new MemoryStream();
			using (IDatabase db = Database.FromFile("aa.db")){//.FromStream(ms, true)) {

				db.Insert("hello", "Hello, World!");

				var pair = db.Get("hello");
				//db.OverwriteValue(pair, "Goodbye, World!");
				Console.WriteLine(pair.Value.GetAs<string>());

				db.InsertRange(new KeyValuePair<string, string>[] {
					new KeyValuePair<string, string>("test1", "Value for 1!"),
					new KeyValuePair<string, string>("test2", "Value for 2!"),
					new KeyValuePair<string, string>("test3", "Value for 3!"),
				});
				
				foreach(var i in db) {
					Console.WriteLine(i);
					Console.WriteLine($"Index's Type: {i.Index.GetTypeOf()}");
					Console.WriteLine($"Value's Type: {i.Index.GetTypeOf()}");
					Console.WriteLine($"Index as a byte array: {i.Index.GetAs<byte[]>()}");
					Console.WriteLine($"Value as a byte array: {i.Value.GetAs<byte[]>()}");
					Console.WriteLine($"Index as a string: {i.Index.GetAs<string>()}");
					Console.WriteLine($"Value as a string: {i.Value.GetAs<string>()}");
				}

				foreach (var i in db)
					Console.WriteLine($"{i.Index.GetAs<string>()}, {i.Value.GetAs<string>()}");

				int max = 10_000;
				var stp = Stopwatch.StartNew();

				for(int i = 0; i < max; i++)
					foreach(var ix in db) {
						for (int k = 0; k < 10; k++)
							ix.Value.GetAs<Stream>();
					}

				stp.Stop();

				Console.WriteLine(stp.ElapsedMilliseconds);

				var pairs = new List<Reader.IReaderPair>();
				foreach (var i in db) pairs.Add(i);

				stp.Restart();

				for(int i = 0; i < max; i++)
					foreach(var ix in pairs) {
						for (int k = 0; k < 10; k++)
							ix.Value.GetAs<Stream>();
					}

				stp.Stop();

				Console.WriteLine(stp.ElapsedMilliseconds);
			}

			Console.ReadLine();

			//var db = Database.FromFile("TEST.db");

			//db.Fill("MEMES", "i like, MEMES", 100);

			//db.Dispose();

			/*
			IDatabase db = Database.FromStream(new MemoryStream(), true);
			Time(20_000, () => {
				db = Database.FromStream(new MemoryStream(), true);
				for (int i = 0; i < 1_000_000; i++) db.Insert("HELLO_,", "WORLD_,");
			}, () => {
			}, () => {
				db.Dispose();
				db = Database.FromStream(new MemoryStream(), true);
			});

			Console.ReadKey(true);

			var strMgr = TypeManager.GetHandlerFor<string>();

			Time(20_000, () => {
				db = Database.FromStream(new MemoryStream(), true);
			}, () => {
				db.Insert(strMgr, strMgr, "HELLO", "WORLD");
			}, () => {
				db.Dispose();
				db = Database.FromStream(new MemoryStream(), true);
			});

			Console.ReadLine();

			return;
			/*
			using (var db = Database.FromStream(new MemoryStream(), true)) {
				db.MakeThreadSafe();
				Parallel.For(0, 1_000_000, (i) => {
					db.Insert(i, i);
				});
			}

			Console.ReadLine();/8/8*/

			/*
			var ind = System.Text.Encoding.UTF8.GetBytes("TEST INDEX");
			var val = System.Text.Encoding.UTF8.GetBytes("TEST VALUE USED FOR PROFILING");

			IEnumerable<KeyValuePair<byte[], byte[]>> insertRange = new KeyValuePair<byte[], byte[]>[] { new KeyValuePair<byte[], byte[]>(ind, val) };

			var byteHandler = TypeManager.GetHandlerFor<byte[]>();

			using (var db = Database.FromFile("test.db"))
				for (int i = 0; i < 10_000_000; i++)
					db.InsertRange(byteHandler, byteHandler, insertRange);

			return;
			*//*
			var ms = new MemoryStream();

			using (var fs = File.Open("test.db", FileMode.OpenOrCreate))
				fs.CopyTo(ms);

			using (var db = Database.FromStream(ms, true))
				for (var i = 0; i < 100_000_000; i++)
					foreach (var j in db) { }*/
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

	public static class Helper {

		public static IEnumerable<T> AsEnumerable<T>(this T item) {
			yield return item;
		}
	}
}