#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6
#define USE_ASSEMBLIES
#endif

//only versions that support reflection ( & longlength )

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StringDB {

	internal static class AutoRegisterHelper {

		public static void RegisterTypes() {
#if USE_ASSEMBLIES
			TypeManager.AutoRegister.RegisterTypes();
#endif
		}
	}

#if USE_ASSEMBLIES
	public static partial class TypeManager {
		/// <summary>Automatically registers the class when the TypeManager is initialized.</summary>
		[AttributeUsage(AttributeTargets.Class)]
		public sealed class AutoRegister : Attribute {
			internal static void RegisterTypes() {
				// look through all the assemblies to find any [AutoRegister] attributes

				// make sure we can use the cool assembly things

				foreach (var assembly in GetDependentAssemblies(typeof(TypeManager).Assembly)) {
					foreach (var type in assembly.GetTypes())
						// if it has the attribute
						if (type.IsDefined(typeof(AutoRegister)))
							if (type.GetInterfaces().Contains(typeof(ITypeHandler))) {
								var typeHandler = (ITypeHandler)Activator.CreateInstance(type);

								TypeManager.RegisterType(typeHandler.Type, typeHandler);
							} else throw new Exception($"The AutoRegister attribute can only go on TypeHandler<T>'s children!");
				}
			}

			private static IEnumerable<Assembly> GetDependentAssemblies(Assembly analyzedAssembly) {
				return AppDomain.CurrentDomain.GetAssemblies()
					.Where(a => GetNamesOfAssembliesReferencedBy(a)
										.Contains(analyzedAssembly.FullName));
			}

			private static IEnumerable<string> GetNamesOfAssembliesReferencedBy(Assembly assembly) {
				return assembly.GetReferencedAssemblies()
					.Select(assemblyName => assemblyName.FullName);
			}
		}
	}
#endif
}