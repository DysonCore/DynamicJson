using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DysonCore.DynamicJson
{
    /// <summary>
    /// Provides internal <see cref="Assembly"/> utility methods.
    /// </summary>
    internal static class AssemblyUtils
    {
        /// <summary>
        /// Returns the runtime <see cref="Assembly"/> of <see cref="DynamicJson"/> package.
        /// </summary>
        internal static Assembly GetPackageRuntimeAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
        
        /// <summary>
        /// Retrieves assemblies that reference the given <see cref="Assembly"/>.
        /// </summary>
        /// <param name="currentAssembly">The <see cref="Assembly"/> to check references for.</param>
        /// <returns>A list of assemblies that reference the provided <see cref="Assembly"/>.</returns>
        internal static Assembly[] GetReferencingAssemblies(this Assembly currentAssembly)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            return loadedAssemblies
                .Where(assembly => assembly.IsReferencing(currentAssembly))
                .ToArray();
        }

        /// <summary>
        /// Checks if the given <see cref="Assembly"/> references another specified target <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to check.</param>
        /// <param name="targetAssembly">The target <see cref="Assembly"/> to find in the references.</param>
        /// <returns>True if the <see cref="Assembly"/> references the target <see cref="Assembly"/>, otherwise false.</returns>
        private static bool IsReferencing(this Assembly assembly, Assembly targetAssembly)
        {
            string targetName = targetAssembly.FullName;
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            HashSet<string> referencedSet = new HashSet<string>(referencedAssemblies.Select(referenced => referenced.FullName));
            return referencedSet.Contains(targetName);
        }
    }
}