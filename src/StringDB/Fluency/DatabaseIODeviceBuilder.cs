using JetBrains.Annotations;

namespace StringDB.Fluency
{
	/// <summary>
	/// A class that allows the extensive usage of extensions to create DatabaseIODevices.
	/// </summary>
	[PublicAPI]
	public struct DatabaseIODeviceBuilder
	{
		public static DatabaseIODeviceBuilder Instance { get; }
	}
}