using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DysonCore.DynamicJson.PolymorphicConverter
{
    /// <summary>
    /// Provides internal <see cref="Assembly"/> utility methods.
    /// </summary>
    internal static class AssemblyUtils
    {
        /// <summary>
        /// Retrieves assemblies that reference the given assembly.
        /// </summary>
        /// <param name="currentAssembly">The assembly to check references for.</param>
        /// <returns>A list of assemblies that reference the provided assembly.</returns>
        internal static Assembly[] GetReferencingAssemblies(this Assembly currentAssembly)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            return loadedAssemblies
                .Where(assembly => assembly.IsReferencing(currentAssembly))
                .ToArray();
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
    }
}