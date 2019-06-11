using JetBrains.Annotations;

using StringDB.Querying.Queries;

using System;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	public class ReactiveQueryManager<TKey, TValue> : IQueryManager<TKey, TValue>
	{
		public void Dispose() => throw new NotImplementedException();

		public Task<bool> ExecuteQuery([NotNull] IQuery<TKey, TValue> query) => throw new NotImplementedException();

		public Task ExecuteQuery([NotNull] IWriteQuery<TKey, TValue> writeQuery) => throw new NotImplementedException();
	}
}