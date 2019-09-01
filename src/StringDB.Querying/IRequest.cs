using JetBrains.Annotations;

using System;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// Describes a request for a value.
	/// </summary>
	/// <typeparam name="TValue">The type of value to request.</typeparam>
	[PublicAPI]
	public interface IRequest<TValue> : IDisposable
	{
		/// <summary>
		/// Submit a request for the value.
		/// This will return a task that will complete once
		/// the value is read.
		/// </summary>
		[NotNull, ItemNotNull]
		Task<TValue> Request();
	}
}