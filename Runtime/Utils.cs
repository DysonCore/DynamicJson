using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DysonCore.PolymorphicJson
{
    /// <summary>
    /// Provides utility methods to support typified JSON functionalities.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Retrieves assemblies that reference the given assembly.
        /// </summary>
        /// <param name="currentAssembly">The assembly to check references for.</param>
        /// <returns>A list of assemblies that reference the provided assembly.</returns>
        internal static List<Assembly> GetReferencingAssemblies(this Assembly currentAssembly)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            return loadedAssemblies
                .Where(assembly => assembly.IsReferencing(currentAssembly))
                .ToList();
        }

        /// <summary>
        /// Checks if the given assembly references another specified target assembly.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <param name="targetAssembly">The target assembly to find in the references.</param>
        /// <returns>True if the assembly references the target assembly, otherwise false.</returns>
        private static bool IsReferencing(this Assembly assembly, Assembly targetAssembly)
        {
            string targetName = targetAssembly.FullName;
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            HashSet<string> referencedSet = new HashSet<string>(referencedAssemblies.Select(referenced => referenced.FullName));
            return referencedSet.Contains(targetName);
        }
        
        /// <summary>
        /// Retrieves the lowest type within an inheritance chain that defines a specific property.
        /// </summary>
        /// <param name="currentType">The starting type from which the search begins.</param>
        /// <param name="propertyName">The name of the property to search for in the inheritance chain.</param>
        /// <returns>The most derived type that defines the specified property. Returns the original <paramref name="currentType"/> if the property is not found in any base type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="currentType"/> is null.</exception>
        /// <remarks>
        /// This method searches the inheritance hierarchy of the provided <paramref name="currentType"/> for a specified property.
        /// It aims to find the "lowest" or most derived type that defines the property. If two types in the inheritance chain have
        /// a property with the same name, the method will return the more derived type. 
        /// </remarks>
        internal static Type GetDefiningType(this Type currentType, string propertyName)
        {
            if (currentType == null)
            {
                throw new ArgumentNullException($"[{nameof(Utils)}.{nameof(GetDefiningType)}] {nameof(currentType)} is null. {nameof(propertyName)} - {propertyName}");
            }
            
            Type definingType = currentType;
            
            Type baseType = currentType.BaseType;

            while (baseType != null)
            {
                PropertyInfo baseProperty = baseType.GetProperty(propertyName);
                if (baseProperty != null)
                {
                    definingType = baseType;
                }

                baseType = baseType.BaseType;
            }

            return definingType;
        }
    }
}