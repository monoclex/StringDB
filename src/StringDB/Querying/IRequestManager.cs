using System;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A gateway between an IRequest and the provider of the value being
	/// requested. This allows a single IRequest to be passed to multiple
	/// places, and provide
	/// </summary>
	/// <typeparam name="TRequestKey">The type of the request key.
	/// Whatever this is, it must be uniquely identifiable so values can be
	/// obtained from it.</typeparam>
	/// <typeparam name="TValue">The type of the item.</typeparam>
	public interface IRequestManager<TRequestKey, TValue> : IDisposable
	{
		/// <summary>
		/// The provider should call this in a loop. The task will complete
		/// when the provider needs to provide a value.
		/// </summary>
		Task<NextRequest<TRequestKey, TValue>> NextRequest();

		/// <summary>
		/// Creates an <see cref="IRequest{TValue}"/>, given the
		/// <see cref="TRequestKey"/>. Hand this IRequest to as many consumers
		/// as possible, and once one of the consumers asks for the value, any
		/// consumer waiting for the value or who is going to request it will
		/// automatically have the value.
		/// </summary>
		/// <param name="requestKey"></param>
		IRequest<TValue> CreateRequest(TRequestKey requestKey);
	}
}
