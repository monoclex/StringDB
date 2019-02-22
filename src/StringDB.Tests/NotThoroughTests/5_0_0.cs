using FluentAssertions;

using StringDB.Databases;
using StringDB.IO;
using StringDB.IO.Compatability;
using StringDB.Transformers;

using System.Collections.Generic;
using System.IO;

using Xunit;

namespace StringDB.Tests.NotThoroughTests
{
	public class _5_0_0
	{
		// TODO: remove the manual testing labor component of this
		// currently you should load a project w/ stringdb 5.0.0 and check if it works
		// just make sure it reads it fine and inserting it is fine

		[Fact]
		public void WorksIGuess()
		{
			var st = new StringTransformer();

			// using (var fs = File.Open("copy.db", FileMode.OpenOrCreate))
			using (var ms = new MemoryStream())
			{
				using (var lowlevelDBIODevice = new StringDB5_0_0LowlevelDatabaseIODevice(ms, true))
				using (var dbIODevice = new DatabaseIODevice(lowlevelDBIODevice))
				using (var iodb = new IODatabase(dbIODevice))
				using (var db = new TransformDatabase<byte[], byte[], string, string>(iodb, st, st))
				{
					db.Insert("test", "value");
					db.InsertRange(new KeyValuePair<string, string>[]
					{
						new KeyValuePair<string, string>("a,", "c,"),
						new KeyValuePair<string, string>("b,", "d,"),
					});

					File.WriteAllBytes("sdb.db", ms.ToArray());

					db.EnumerateAggresively(2)
						.Should()
						.BeEquivalentTo
						(
							new KeyValuePair<string, string>[]
							{
								new KeyValuePair<string, string>("test", "value"),
								new KeyValuePair<string, string>("a,", "c,"),
								new KeyValuePair<string, string>("b,", "d,"),
							}
						);
				}

				ms.Seek(0, SeekOrigin.Begin);
				// ms.CopyTo(fs);
			}
		}
	}
}