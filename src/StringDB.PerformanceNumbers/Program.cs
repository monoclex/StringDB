using BenchmarkDotNet.Running;
using StringDB.Fluency;
using StringDB.IO;
using StringDB.Querying;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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
		/*
			var buffer = new BufferBlock<int>();

			var producer = Task.Run(() =>
			{
				int i = 0;

				while(true)
				{
					buffer.Post(i++);

					if (i == int.MaxValue)
					{
						i = int.MinValue;
					}
				}
			});

			int cps = 0;

			var consumer = Task.Run(async () =>
			{
				while(true)
				{
					await buffer.ReceiveAsync().ConfigureAwait(false);

					cps++;
				}
			});

			var counter = Task.Run(async () =>
			{
				while(true)
				{
					await Task.Delay(1000);
					Console.WriteLine(cps);
					cps = 0;
				}
			});

			await Task.WhenAll(producer, consumer, counter);
		*/
			var db = new DatabaseBuilder()
				.UseIODatabase(StringDBVersion.Latest, "ZFWorlds.db");

				/*
			Console.WriteLine(db.Count());

			int cps = 0;

			var counter = Task.Run(async () =>
			{
				while (true)
				{
					await Task.Delay(1000);
					Console.WriteLine(cps);
					cps = 0;
				}
			});

			var reader = Task.Run(async () =>
			{
				while (true)
				{
					foreach(var kvp in db)
					{
						cps++;
					}
				}
			});

			await Task.WhenAll(counter, reader);
			*/
			var queryManager = new QueryManager<byte[], byte[]>(db);

			var ips = 0;

			var q = queryManager.ExecuteQuery(new Querying.Queries.Query<byte[], byte[]>(async (a, b) =>
			{
				ips++;
				return QueryAcceptance.Continue;
			}));

			while (true)
			{
				await Task.Delay(1000);

				Console.WriteLine(ips);

				ips = 0;
			}

			Console.ReadLine();


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