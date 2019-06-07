using StringDB.Querying;
using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Tests.QueryingTests
{
	public class QueryTests : IDisposable
	{
		private IQuery<int, int> _query;



		public void Dispose() => _query.Dispose();
	}
}
