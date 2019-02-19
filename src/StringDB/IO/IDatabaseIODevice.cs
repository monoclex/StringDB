using System;
using System.Collections.Generic;

namespace StringDB.IO
{
	public interface IDatabaseIODevice : IDisposable
	{
		void Reset();

		DatabaseItem ReadNext();

		byte[] ReadValue(long position);

		void Insert(KeyValuePair<byte[], byte[]>[] items);
	}
}