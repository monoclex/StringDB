using StringDB.DBTypes.Predefined;

using System;
using System.Collections.Generic;

namespace StringDB.DBTypes {

	/// <summary>Manages the types StringDB can read and write. Add your own if you need more types!</summary>
	public static class TypeManager {

		static TypeManager() {
			TypeHandlers = new Dictionary<Type, ITypeHandler>();

			RegisterType(new ByteArrayType());
			RegisterType(new StringType());
			RegisterType(new StreamType());
		}

		private static Dictionary<Type, ITypeHandler> TypeHandlers { get; set; }
		private static readonly object _locker = new object(); // thread safeness since it's a static class

		/// <summary>Register a type</summary>
		/// <param name="t">The type to register</param>
		public static void RegisterType<T>(TypeHandler<T> t) {
			lock (_locker) {
				if (TypeHandlers.TryGetValue(typeof(T), out var _)) // check if the type itself exists
					throw new TypeHandlerExists(typeof(T));

				if (UniqueByteIdExists(t)) throw new TypeHandlerExists(typeof(T)); // make sure the unique byte handler doesn't exist
				else TypeHandlers.Add(typeof(T), t);
			}
		}

		/// <summary>Overrides an existing type register, or adds it if it doesn't exist.</summary>
		/// <param name="t">The type to override</param>
		public static void OverridingRegisterType<T>(TypeHandler<T> t) {
			lock (_locker) {
				if (UniqueByteIdExists(t)) throw new TypeHandlerExists(typeof(T)); // make sure the unique byte handler doesn't exist

				TypeHandlers[typeof(T)] = t;
			}
		}

		/// <summary>Get the type handler for a type</summary>
		/// <typeparam name="T">The type of type handler</typeparam>
		public static TypeHandler<T> GetHandlerFor<T>() {
			lock (_locker) {
				if (!TypeHandlers.TryGetValue(typeof(T), out var handler)) throw new TypeHandlerDoesntExist(typeof(T));

				return handler as TypeHandler<T>;
			}
		}

		/// <summary>Returns the TypeHandler given a unique byte identifier.</summary>
		/// <param name="id">The TypeHandler for the given byte Id</param>
		public static ITypeHandler GetHandlerFor(byte id) {
			lock (_locker) {
				foreach (var i in TypeHandlers)
					if (i.Value.Id == id)
						return i.Value;
				throw new TypeHandlerDoesntExist(id);
			}
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