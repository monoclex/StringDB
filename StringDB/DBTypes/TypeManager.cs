#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6
#define USE_ASSEMBLIES
#endif

using StringDB.DBTypes.Predefined;
using StringDB.Exceptions;

using System;
using System.Collections.Concurrent;

namespace StringDB {
	//TODO: mild cleanup with USE_ASSEMBLIES

	/// <summary>Manages the types StringDB can read and write. Add your own if you need more types!</summary>
	public static partial class TypeManager {

		static TypeManager() {
			lock (_initLock)
				if (_shouldInit) {
					_shouldInit = false;
					TypeHandlers = new ConcurrentDictionary<Type, ITypeHandler>();

					RegisterType(new ByteArrayType());
					RegisterType(new StringType());
					RegisterType(new StreamType());

#if USE_ASSEMBLIES
					AutoRegister.RegisterTypes();
#endif
				}
		}

		private static readonly object _initLock = new object();
		private static readonly bool _shouldInit = true;
		private static ConcurrentDictionary<Type, ITypeHandler> TypeHandlers { get; set; }

		/// <summary>Register a type</summary>
		/// <param name="t">The type to register</param>
		public static void RegisterType<T>(TypeHandler<T> t) => RegisterType(typeof(T), t);

		private static void RegisterType(Type t, ITypeHandler typeHandler) {
			if (!TypeHandlers.TryAdd(t, typeHandler))
				throw new TypeHandlerExists(t);

			var tmp = TypeHandlers;

			foreach (var i in tmp)
				if (i.Value.Id == typeHandler.Id && i.Key != t)
					if (TypeHandlers.TryRemove(t, out var _))
						throw new TypeHandlerExists(t);
		}

		/// <summary>Overrides an existing type register, or adds it if it doesn't exist.</summary>
		/// <param name="t">The type to override</param>
		public static void OverridingRegisterType<T>(TypeHandler<T> t) {
			if (UniqueByteIdExists(t)) throw new TypeHandlerExists(typeof(T)); // make sure the unique byte handler doesn't exist

			TypeHandlers[typeof(T)] = t;
		}

		/// <summary>Get the type handler for a type</summary>
		/// <typeparam name="T">The type of type handler</typeparam>
		public static TypeHandler<T> GetHandlerFor<T>() {
			if (!TypeHandlers.TryGetValue(typeof(T), out var handler)) throw new TypeHandlerDoesntExist(typeof(T));

			return handler as TypeHandler<T>;
		}

		/// <summary>Returns the TypeHandler given a unique byte identifier.</summary>
		/// <param name="id">The TypeHandler for the given byte Id</param>
		internal static ITypeHandler GetHandlerFor(byte id) {
			foreach (var i in TypeHandlers)
				if (i.Value.Id == id)
					return i.Value;
			throw new TypeHandlerDoesntExist(id);
		}

		private static bool UniqueByteIdExists<T>(TypeHandler<T> t) {
			try { // check if the unique byte identifier exists
				GetHandlerFor(t.Id);
				return true;
			} catch (TypeHandlerDoesntExist) {
				return false;
			}
		}
	}
}

namespace StringDB.Exceptions {

	/// <summary>An exception that gets thrown when attempting to register a Type if it already exists</summary>
	public class TypeHandlerExists : Exception { internal TypeHandlerExists(Type t) : base($"The TypeHandler already exists ({t}). See OverridingRegisterType if you'd like to override existing types.") { } }

	/// <summary>An exception that gets thrown when attempting to get a Type that doesn't exist.</summary>
	public class TypeHandlerDoesntExist : Exception { internal TypeHandlerDoesntExist(object t) : base($"The TypeHandler ({t}) doesn't exist. See RegisterType if you'd like to add a type, or if you're trying to read from this database, then there are missing TypeHandlers.") { } }
}