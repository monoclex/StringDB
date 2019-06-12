using BenchmarkDotNet.Running;

using System;

namespace StringDB.PerformanceNumbers
{
	public static class Program
	{
		public enum BenchmarkToRun
		{
			YieldOrLinq
		}

		private static void Main()
		{
			const BenchmarkToRun benchmark = BenchmarkToRun.YieldOrLinq;

			var summary = BenchmarkRunner.Run(GetBenchmarkType(benchmark));

			new SingleInsertFileSize().Run();
			new InsertRangeFileSize().Run();

			Console.WriteLine(summary);

			Console.ReadLine();
		}

		private static Type GetBenchmarkType(BenchmarkToRun benchmark)
		{
			switch (benchmark)
			{
				case BenchmarkToRun.YieldOrLinq: return typeof(YieldOrLinq);
				default: throw new Exception("");
			}
		}
	}
}