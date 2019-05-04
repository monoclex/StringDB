using Ceras;

using JetBrains.Annotations;

using System.Runtime.CompilerServices;

namespace StringDB.Ceras
{
	/// <summary>
	/// Use Ceras as a serializer and deserializer for byte arrays.
	/// </summary>
	/// <typeparam name="T">The type of object to store.</typeparam>
	[PublicAPI]
	public class CerasTransformer<T> : ITransformer<byte[], T>
	{
		/// <summary>
		/// A default, global instance of this kind of transformer
		/// that uses the CerasSerializer at <see cref="CerasTransformerExtensions.CerasInstance"/>.
		/// </summary>
		public static CerasTransformer<T> Default { get; } = new CerasTransformer<T>(CerasTransformerExtensions.CerasInstance);

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