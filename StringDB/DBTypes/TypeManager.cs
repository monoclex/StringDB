using StringDB.DBTypes.Predefined;

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace StringDB.DBTypes {

	/// <summary>Manages the types StringDB can read and write. Add your own if you need more types!</summary>
	public static class TypeManager {

		static TypeManager() {
			TypeHandlers = new ConcurrentDictionary<Type, ITypeHandler>();

			RegisterType(new ByteArrayType(), int.MaxValue);
			RegisterType(new StringType());
			RegisterType(new StreamType());
		}

		private static ConcurrentDictionary<Type, ITypeHandler> TypeHandlers { get; set; }

		/// <summary>Register a type</summary>
		/// <param name="t">The type to register</param>
		/// <param name="maxTries">The maximum number of tries that should happen before throwing an InvalidOperationException. Set it to 0 to try to repeatedly add until it can't.</param>
		public static void RegisterType<T>(TypeHandler<T> t, int maxTries = 10) {
			if (!TypeHandlers.TryAdd(typeof(T), t))
				throw new TypeHandlerExists(typeof(T));

			var tmp = TypeHandlers;

			foreach (var i in tmp)
				if (i.Value.Id == t.Id)
					if(TypeHandlers.TryRemove(typeof(T), out var _))
						throw new TypeHandlerExists(typeof(T));
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
		public static ITypeHandler GetHandlerFor(byte id) {
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
	
	/// <summary>An exception that gets thrown when attempting to register a Type if it already exists</summary>
	public class TypeHandlerExists : Exception { internal TypeHandlerExists(Type t) : base($"The TypeHandler already exists ({t}). See OverridingRegisterType if you'd like to override existing types.") { } }

	/// <summary>An exception that gets thrown when attempting to get a Type that doesn't exist.</summary>
	public class TypeHandlerDoesntExist : Exception { internal TypeHandlerDoesntExist(object t) : base($"The TypeHandler ({t}) doesn't exist. See RegisterType if you'd like to add a type, or if you're trying to read from this database, then there are missing TypeHandlers.") { } }
}