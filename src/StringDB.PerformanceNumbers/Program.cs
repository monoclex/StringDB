using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LiteDB;
using StringDB.IO.Compatibility;
using FileMode = System.IO.FileMode;

namespace StringDB.PerformanceNumbers
{
	class Program
	{
		static void Main(string[] args)
		{
			new SingleInsertFileSize().Run();
			new InsertRangeFileSize().Run();

			Console.ReadLine();
		}
	}
}
