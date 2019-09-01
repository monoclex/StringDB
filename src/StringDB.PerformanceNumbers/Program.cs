using BenchmarkDotNet.Running;

using System;
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