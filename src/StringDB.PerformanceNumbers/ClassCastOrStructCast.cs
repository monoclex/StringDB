using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.PerformanceNumbers
{
	// | Method |     Mean |     Error |    StdDev |
	// |------- |---------:|----------:|----------:|
	// |  Class | 3.393 ns | 0.0572 ns | 0.0507 ns |
	// | Struct | 3.872 ns | 0.0790 ns | 0.0739 ns |

	public interface IThing
	{
		int Number { get; }
	}

	public class ClassThing : IThing
	{
		public int Number => 1;
	}

	public struct StructThing : IThing
	{
		public int Number => 1;
	}

	public class ClassCastOrStructCast
	{
		[Benchmark]
		public IThing Class()
		{
			return new ClassThing();
		}

		[Benchmark]
		public IThing Struct()
		{
			return new StructThing();
		}
	}
}
