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

		internal static Dictionary<Type, ITypeHandler> TypeHandlers { get; private set; }
		internal static readonly object Locker = new object();

		/// <summary>Register a type</summary>
		/// <param name="t">The type to register</param>
		public static void RegisterType<T>(TypeHandler<T> t) {
			lock (Locker) {
				if (TypeHandlers.TryGetValue(typeof(T), out var _))
					throw new TypeHandlerExists();

				try {
					GetHandlerFor(t.Id);
					throw new TypeHandlerExists();
				} catch (TypeHandlerDoesntExist) {
					TypeHandlers.Add(typeof(T), t);
				}
			}
		}

		/// <summary>Overrides an existing type register, or adds it if it doesn't exist.</summary>
		/// <param name="t">The type to override</param>
		public static void OverridingRegisterType<T>(TypeHandler<T> t) {
			lock (Locker) {
				TypeHandlers[typeof(T)] = t;
			}
		}

		/// <summary>Get the type handler for a type</summary>
		/// <typeparam name="T">The type of type handler</typeparam>
		public static TypeHandler<T> GetHandlerFor<T>() {
			lock (Locker) {
				if (!TypeHandlers.TryGetValue(typeof(T), out var handler)) throw new TypeHandlerDoesntExist();

				return handler as TypeHandler<T>;
			}
		}

		/// <summary>Returns the TypeHandler given a unique byte identifier.</summary>
		/// <param name="id">The TypeHandler for the given byte Id</param>
		public static ITypeHandler GetHandlerFor(byte id) {
			lock (Locker) {
				foreach (var i in TypeHandlers)
					if (i.Value.Id == id)
						return i.Value;
				throw new TypeHandlerDoesntExist();
			}
		}
	}

	/// <summary>An exception that gets thrown when attempting to register a Type if it already exists</summary>
	public class TypeHandlerExists : Exception { internal TypeHandlerExists() : base("The TypeHandler already exists. See OverridingRegisterType if you'd like to override existing types.") { } }

	/// <summary>An exception that gets thrown when attempting to get a Type that doesn't exist.</summary>
	public class TypeHandlerDoesntExist : Exception { internal TypeHandlerDoesntExist() : base("The TypeHandler doesn't exist. See RegisterType if you'd like to add a type, or if you're trying to read from this database, then there are missing TypeHandlers!.") { } }
}