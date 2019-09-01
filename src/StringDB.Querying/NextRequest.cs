using JetBrains.Annotations;
using System;

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

		[NotNull]
		public TRequestKey RequestKey { get; }

		[NotNull]
		public Action<TValue> SupplyValue { get; }
	}
}