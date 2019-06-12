using JetBrains.Annotations;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Utility functions for parallel enumeration.
	/// </summary>
	[PublicAPI]
	public static class ParallelAsync
	{
		// https://devblogs.microsoft.com/pfxteam/implementing-a-simple-foreachasync-part-2/
		[NotNull, ItemCanBeNull]
		public static async Task<TResult> ForEachAsync<T, TResult>
		(
			[NotNull] this IEnumerable<T> source,
			[NotNull, ItemNotNull] Func<T, ParallelAsyncController<TResult>, Task> body,
			int degreeOfParallelism = -1
		)
		{
			if (degreeOfParallelism == -1)
			{
				degreeOfParallelism = Environment.ProcessorCount;
			}

			var cts = new CancellationTokenSource();
			var controller = new ParallelAsyncController<TResult>(cts);

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
							}
						});
					})
			).ConfigureAwait(false);

			return controller.Result;
		}
	}
}