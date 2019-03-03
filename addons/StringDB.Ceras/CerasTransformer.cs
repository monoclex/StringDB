using Ceras;

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace StringDB.Ceras
{
	/// <summary>
	/// Use Ceras as a serializer and deserializer for byte arrays.
	/// </summary>
	/// <typeparam name="T">The type of object to store.</typeparam>
	[PublicAPI]
	public class CerasTransformer<T> : ITransformer<byte[], T>
	{
		private readonly CerasSerializer _ceras;

		/// <summary>
		/// Creates a new CerasTransformer.
		/// </summary>
		/// <param name="ceras">The serializer to use.</param>
		public CerasTransformer([NotNull] CerasSerializer ceras) => _ceras = ceras;

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T TransformPre(byte[] pre) => _ceras.Deserialize<T>(pre);

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] TransformPost(T post) => _ceras.Serialize<T>(post);
	}
}