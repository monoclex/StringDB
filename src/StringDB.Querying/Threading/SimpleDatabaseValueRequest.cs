using System.Threading;
using System.Threading.Tasks;

namespace StringDB.Querying.Threading
{
	public class SimpleDatabaseValueRequest<TValue> : IRequest<TValue>
	{
		private readonly ILazyLoader<TValue> _lazyLoader;
		private readonly RequestLock _databaseLock;

		public SimpleDatabaseValueRequest
		(
			ILazyLoader<TValue> lazyLoader,
			RequestLock databaseLock
		)
		{
			_lazyLoader = lazyLoader;
			_databaseLock = databaseLock;
		}

		private readonly SemaphoreSlim _thisLock = new SemaphoreSlim(1);
		private TValue _cachedValue;
		private bool _cached;

		public async Task<TValue> Request()
		{
			if (_cached)
			{
				return _cachedValue;
			}

			await _thisLock.WaitAsync()
				.ConfigureAwait(false);

			try
			{
				if (_cached)
				{
					return _cachedValue;
				}

				// need to access the db

				await _databaseLock.RequestAsync()
					.ConfigureAwait(false);

				try
				{
					_cachedValue = _lazyLoader.Load();
					_cached = true;
					return _cachedValue;
				}
				finally
				{
					_databaseLock.Release();
				}
			}
			finally
			{
				_thisLock.Release();
			}
		}

		public void Dispose() => _thisLock.Dispose();
	}
}