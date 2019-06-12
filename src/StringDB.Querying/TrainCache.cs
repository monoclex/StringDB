using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace StringDB.Querying
{
	public class TrainCache<T>
	{
		public object Lock;
		public int Accessors;
		public T Item;
	}
}