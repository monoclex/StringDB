using BenchmarkDotNet.Running;
using StringDB.Querying;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StringDB.PerformanceNumbers
{
	public static class Program
	{
		public enum BenchmarkToRun
		{
			YieldOrLinq,
			ClassCastOrStructCast
		}

		private static async Task Main()
		{
			Console.WriteLine("new");
			using (var db = StringDatabase.Create())
			{
				var qm = db.NewQueryManager();

				db.Insert("t", "m");

				var result = await qm.Find(x => x == "t").ConfigureAwait(false);

				Console.WriteLine(result);
			}

			new SingleInsertFileSize().Run();
			new InsertRangeFileSize().Run();
			await new MessagePipeMessagesPerSecond().Run().ConfigureAwait(false);

			const BenchmarkToRun benchmark = BenchmarkToRun.ClassCastOrStructCast;

			var summary = BenchmarkRunner.Run(GetBenchmarkType(benchmark));

			Console.WriteLine(summary);

			Console.ReadLine();
		}

		private static Type GetBenchmarkType(BenchmarkToRun benchmark)
		{
			switch (benchmark)
			{
				case BenchmarkToRun.YieldOrLinq: return typeof(YieldOrLinq);
				case BenchmarkToRun.ClassCastOrStructCast: return typeof(ClassCastOrStructCast);
				default: throw new Exception("");
			}
		}
	}
}