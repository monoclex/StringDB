using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A very simple utility class to return in the
	/// <see cref="IRequestManager{TRequestKey, TValue}.Disable"/> method of an
	/// <see cref="IRequestManager{TRequestKey, TValue}"/>.
	/// </summary>
	[PublicAPI]
	public sealed class DisableRequestManager : IDisposable
	{
		private readonly TaskCompletionSource<bool> _tcs;
		private readonly Action _onDispose;

		[NotNull]
		public Task WhenEnabled { get; }

		public DisableRequestManager(Action onDispose)
		{
			_tcs = new TaskCompletionSource<bool>();
			WhenEnabled = _tcs.Task;
			_onDispose = onDispose;
		}

		public void Dispose()
		{
			_onDispose();
			_tcs.SetResult(true);
		}
	}
}
