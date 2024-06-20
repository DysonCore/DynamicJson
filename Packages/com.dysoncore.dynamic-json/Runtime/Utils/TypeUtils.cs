using System;
using System.Reflection;

namespace DysonCore.DynamicJson
{
    /// <summary>
    /// Provides internal <see cref="Type"/> utility methods.
    /// </summary>
    internal static class TypeUtils
    {
        /// <summary>
        /// Retrieves the lowest type within an inheritance chain which declares a specific property.
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
        internal static Type GetDeclaringClass(this Type currentType, string propertyName)
        {
            if (currentType == null)
            {
                throw new ArgumentNullException($"[{nameof(TypeUtils)}.{nameof(GetDeclaringClass)}] {nameof(currentType)} is null. {nameof(propertyName)} - {propertyName}");
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