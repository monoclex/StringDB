using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public class ParallelAsyncController<TResult>
	{
		private readonly CancellationTokenSource _cancellationTokenSource;

		public ParallelAsyncController(CancellationTokenSource cancellationTokenSource)
		{
			_cancellationTokenSource = cancellationTokenSource;
			Result = default;
		}

		public void Stop()
		{
			_cancellationTokenSource.Cancel();
		}

		public TResult Result { get; private set; }

		public void ProvideResult(TResult result)
		{
			Result = result;
		}
	}

	public static class ParallelAsync
	{
		// https://devblogs.microsoft.com/pfxteam/implementing-a-simple-foreachasync-part-2/
		public static async Task<TResult> ForEachAsync<T, TResult>
		(
			this IEnumerable<T> source,
			Func<T, ParallelAsyncController<TResult>, Task> body,
			int degreeOfParallelism = -1
		)
		{
			if (degreeOfParallelism == -1)
			{
				degreeOfParallelism = 1; //  Environment.ProcessorCount;
			}

			var cts = new CancellationTokenSource();
			var controller = new ParallelAsyncController<TResult>(cts);

			Console.WriteLine("parallel start");

			using (var enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					await body(enumerator.Current, controller)
						.ConfigureAwait(false);
				}
				Console.WriteLine("p1 []] parallel finish");
			}

			Console.WriteLine("[][][][][][][][][][][][][][[[][]]] parallel finish");

			/*
			await Task.WhenAll
			(
				Partitioner.Create(source)
					.GetPartitions(degreeOfParallelism)
					.Select(partition =>
					{
						return Task.Run(async () =>
						{
							using (partition)
							{
								while (partition.MoveNext() && !cts.IsCancellationRequested)
								{
									await body(partition.Current, controller)
										.ConfigureAwait(false);
								}

								Console.WriteLine("parallel body stopped");
							}

							Console.WriteLine("a parallel task finished");
						});
					})
			).ConfigureAwait(false);
			// */

			return controller.Result;
		}
	}
}