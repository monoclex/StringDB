using JetBrains.Annotations;

using System;
using System.Collections.Generic;

namespace StringDB.IO
{
	/// <summary>
	/// An IO Device for a database.
	/// </summary>
	[PublicAPI]
	public interface IDatabaseIODevice : IDisposable
	{
		/// <summary>
		/// Resets the device back to the start of reading.
		/// </summary>
		void Reset();

		/// <summary>
		/// Reads the next item in the database.
		/// </summary>
		/// <returns>A database item.</returns>
		DatabaseItem ReadNext();

		/// <summary>
		/// Reads the value specified at a position;
		/// typically gotten from the result of a <see cref="DatabaseItem"/> returned by <seealso cref="ReadNext"/>.
		/// </summary>
		/// <param name="position">The position to begin reading the value from.</param>
		/// <returns>A byte[] with the data.</returns>
		[NotNull]
		byte[] ReadValue(long position);

		/// <summary>
		/// Inserts data into the database.
		/// </summary>
		/// <param name="items">The items to insert.</param>
		void Insert([NotNull] KeyValuePair<byte[], byte[]>[] items);

		/// <summary>
		/// A token source that dictates the optimal time to start reading values.
		/// </summary>
		[NotNull]
		IOptimalTokenSource OptimalTokenSource { get; }
	}
}