using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LiteDB;
using StringDB.Fluency;
using StringDB.IO;
using StringDB.IO.Compatibility;
using StringDB.Transformers;
using FileMode = System.IO.FileMode;

namespace StringDB.PerformanceNumbers
{
	class Program
	{
		public enum BenchmarkToRun
		{
			YieldOrLinq
		}

		static void Main(string[] args)
		{
			using (var db = new DatabaseBuilder()
				.UseIODatabase((builder) => builder.UseStringDB(StringDBVersions.Latest, new MemoryStream()))
				.WithTransform(new StringTransformer(), new StringTransformer()))
			{
				while (true)
				{
					db.Insert("i am profiling", "this code");
				}
			}

			var benchmark = BenchmarkToRun.YieldOrLinq;

			var summary = BenchmarkRunner.Run(GetBenchmarkType(benchmark));

			new SingleInsertFileSize().Run();
			new InsertRangeFileSize().Run();

			Console.ReadLine();
		}

		static Type GetBenchmarkType(BenchmarkToRun benchmark)
		{
			switch (benchmark)
			{
				case BenchmarkToRun.YieldOrLinq: return typeof(YieldOrLinq);
				default: throw new Exception("");
			}
		}
	}
}
