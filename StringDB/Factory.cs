using StringDB.Reader;
using StringDB.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringDB
{
    internal static class Factory
    {
		public static StreamIO GetStreamIO(this Stream input)
			=> new StreamIO(input);

		public static IReader GetReader(this StreamIO input)
			=> new Reader.Reader(input, Factory.GetRawReader(input));

		public static IWriter GetWriter(this StreamIO input)
			=> new Writer.Writer(Factory.GetRawWriter(input));

		public static IReader GetTSReader(this StreamIO input, object @lock)
			=> new Reader.Reader(input, Factory.GetTSRawReader(input, @lock));

		public static IWriter GetTSWriter(this StreamIO input, object @lock)
			=> new Writer.Writer(Factory.GetTSRawWriter(input, @lock));

		public static IRawReader GetTSRawReader(this StreamIO input, object @lock)
			=> new ThreadSafeRawReader(Factory.GetRawReader(input), @lock);

		public static IRawWriter GetTSRawWriter(this StreamIO input, object @lock)
			=> new ThreadSafeRawWriter(Factory.GetRawWriter(input), @lock);

		public static IRawReader GetRawReader(this StreamIO input)
			=> new RawReader(input);

		public static IRawWriter GetRawWriter(this StreamIO input)
			=> new RawWriter(input);
	}
}
