using StringDB;

using System;

namespace _01_GettingStarted
{
	// Just an extremely simple example using the StringDatabase helper

	internal class Program
	{
		private static void Main(string[] args)
		{
			// this is the most simplest way to use StringDB
			// this creates a database in RAM
			using (var db = StringDatabase.Create())
			{
				// we can insert stuff
				db.Insert("key", "Hello, World!");

				// get that value
				var value = db.Get("key");

				// write it to the console
				Console.WriteLine(value);
			}
		}
	}
}