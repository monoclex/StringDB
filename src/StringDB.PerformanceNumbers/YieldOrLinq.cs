using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace StringDB.PerformanceNumbers
{
	/*
	 *	|            Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
	 *	|------------------ |----------:|----------:|----------:|------:|--------:|
	 *	|     BenchmarkLinq | 213.34 ns | 2.0946 ns | 1.9593 ns |  2.35 |    0.03 |
	 *	|    BenchmarkYield |  90.93 ns | 0.8003 ns | 0.7094 ns |  1.00 |    0.00 |
	 *	| CustomIEnumerator |  77.81 ns | 1.1261 ns | 1.0534 ns |  0.86 |    0.01 |
	 */
	public class YieldOrLinq
	{
		public void PrintResults()
		{
			Console.WriteLine("linq:");
			foreach (var item in UseLinq())
			{
				Console.WriteLine(item);
			}

			Console.WriteLine("yield");
			foreach (var item in UseYield())
			{
				Console.WriteLine(item);
			}

			Console.WriteLine("custom");
			foreach (var item in new CustomEnumerator(Evaluate()))
			{
				Console.WriteLine(item);
			}
		}

		[Benchmark]
		public void BenchmarkLinq()
		{
			foreach (var item in UseLinq())
			{
			}
		}

		[Benchmark(Baseline = true)]
		public void BenchmarkYield()
		{
			foreach (var item in UseYield())
			{
			}
		}

		[Benchmark]
		public void CustomIEnumerator()
		{
			foreach (var item in new CustomEnumerator(Evaluate()))
			{
			}
		}

		public IEnumerable<int> UseYield()
		{
			foreach (var item in Evaluate())
			{
				if (item.Item2 == 2)
				{
					yield return item.Item1;
				}
			}
		}

		public IEnumerable<int> UseLinq()
			=> Evaluate()
				.Where(x => x.Item2 == 2)
				.Select(x => x.Item1);

		public IEnumerable<(int, int)> Evaluate()
		{
			yield return (0, 1);
			yield return (1, 2);
			yield return (2, 3);
			yield return (3, 2);
			yield return (4, 1);
		}
	}

	public class CustomEnumerator : IEnumerator<int>, IEnumerable<int>
	{
		private readonly IEnumerable<(int, int)> _enumerateOver;
		private readonly IEnumerator<(int, int)> _enumerator;

		public CustomEnumerator(IEnumerable<(int, int)> enumerateOver)
		{
			_enumerateOver = enumerateOver;
			_enumerator = enumerateOver.GetEnumerator();
		}

		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				var current = _enumerator.Current;

				if (current.Item2 == 2)
				{
					Current = current.Item1;
					return true;
				}
			}

			return false;
		}

		public void Reset() => _enumerator.Reset();

		public int Current { get; private set; }

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			_enumerator.Dispose();
		}

		public IEnumerator<int> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
