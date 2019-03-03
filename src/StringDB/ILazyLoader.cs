using JetBrains.Annotations;

namespace StringDB
{
	/// <summary>
	/// A promise to return a value upon loading.
	/// </summary>
	/// <typeparam name="T">The type of value to return.</typeparam>
	[PublicAPI]
	public interface ILazyLoader<out T>
	{
		/// <summary>
		/// Loads the value that was promised.
		/// </summary>
		/// <returns>The value it loads.</returns>
		[NotNull] T Load();
	}
}