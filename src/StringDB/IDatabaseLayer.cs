using JetBrains.Annotations;

namespace StringDB
{
	/// <summary>
	/// Represents an object that contains a database inside of it.
	/// </summary>
	/// <typeparam name="TKey">The type of key of the database.</typeparam>
	/// <typeparam name="TValue">The type of value of the database.</typeparam>
	[PublicAPI]
	public interface IDatabaseLayer<TKey, TValue>
	{
		/// <summary>
		/// The underlying database that's in use.
		/// </summary>
		[NotNull]
		IDatabase<TKey, TValue> InnerDatabase { get; }
	}
}