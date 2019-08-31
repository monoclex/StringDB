using System;
using System.Threading.Tasks;

namespace StringDB.Querying
{
	/// <summary>
	/// A simple struct to encompass the variables necessary to allow the
	/// provider to provide values to requests.
	/// </summary>
	/// <typeparam name="TRequestKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public struct NextRequest<TRequestKey, TValue>
	{
		public NextRequest
		(
			TRequestKey requestKey,
			Action<TValue> supplyValue
		)
		{
			RequestKey = requestKey;
			SupplyValue = supplyValue;
		}

		public TRequestKey RequestKey { get; }
		public Action<TValue> SupplyValue { get; }
	}
}
