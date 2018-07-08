using System.Collections.Generic;
using StringDB.DBTypes.Predefined;
using System;

namespace StringDB.DBTypes {
	/// <summary>Manages the types StringDB can read and write. Add your own if you need more types!</summary>
	public static class TypeManager {
		static TypeManager() {
			Types = new Dictionary<Type, object>();

			RegisterType(new ByteArrayType());
			RegisterType(new StringType());
			RegisterType(new StreamType());
		}

		internal static Dictionary<Type, object> Types { get; private set; }
		internal static readonly object Locker = new object();

		/// <summary>Register a type</summary>
		/// <param name="t">The type to register</param>
		public static void RegisterType<T>(TypeHandler<T> t) {
			lock (Locker) {
				if (Types.TryGetValue(typeof(T), out var _))
					throw new TypeHandlerExists();

				Types.Add(typeof(T), t);
			}
		}

		/// <summary>Overrides an existing type register, or adds it if it doesn't exist.</summary>
		/// <param name="t">The type to override</param>
		public static void OverridingRegisterType<T>(TypeHandler<T> t) {
			lock (Locker) {
				Types[typeof(T)] = t;
			}
		}

		/// <summary>Get the type handler for a type</summary>
		/// <typeparam name="T">The type of type handler</typeparam>
		public static TypeHandler<T> GetHandlerFor<T>() {
			lock (Locker) {
				if (!Types.TryGetValue(typeof(T), out var handler)) throw new TypeHandlerExists();

				return handler as TypeHandler<T>;
			}
		}
	}

	public class TypeHandlerExists : Exception { public TypeHandlerExists() : base("The TypeHandler already exists. See OverridingRegisterType if you'd like to override existing types.") { } }
	public class TypeHandlerDoesntExist : Exception { public TypeHandlerDoesntExist() : base("The TypeHandler doesn't exist. See RegisterType if you'd like to add a type.") { } }
}