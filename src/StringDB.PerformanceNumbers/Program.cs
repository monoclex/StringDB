using BenchmarkDotNet.Running;

using System;
using System.Threading.Tasks;

namespace StringDB.PerformanceNumbers
{
	public static class Program
	{
		public enum BenchmarkToRun
		{
			YieldOrLinq
		}

		private static async Task Main()
		{
			new SingleInsertFileSize().Run();
			new InsertRangeFileSize().Run();
			await new MessagePipeMessagesPerSecond().Run();

			const BenchmarkToRun benchmark = BenchmarkToRun.YieldOrLinq;

			var summary = BenchmarkRunner.Run(GetBenchmarkType(benchmark));

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