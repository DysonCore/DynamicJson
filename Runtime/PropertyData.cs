using System;
using System.Collections.Generic;

namespace DysonCore.PolymorphicJson
{
    /// <summary>
    /// Represents metadata for properties marked with the <see cref="TypifyingPropertyAttribute"/>. 
    /// Contains information about property type, name, JSON name, and the mapping between values and their respective types.
    /// </summary>
    internal class PropertyData
    {
        public Type PropertyType { get; }
        public string PropertyName { get; }
        public string JsonName { get; }
        public Dictionary<object, Type> ValuesData { get; }

        internal PropertyData(Type propertyType, string propertyName, string jsonName)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            JsonName = string.IsNullOrWhiteSpace(jsonName) ? propertyName : jsonName;
            ValuesData = new Dictionary<object, Type>();
        }
    }
}