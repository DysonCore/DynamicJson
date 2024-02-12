using System;

namespace DysonCore.DynamicJson.PolymorphicConverter
{
    /// <summary>
    /// Base class representing metadata for properties used by polymorphic converter. 
    /// Contains information about property type, name, JSON name.
    /// </summary>
    internal abstract class PropertyData
    {
        internal Type PropertyType { get; }
        internal string PropertyName { get; }
        internal string JsonName { get; }

        protected PropertyData(Type propertyType, string propertyName, string jsonName)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            JsonName = string.IsNullOrWhiteSpace(jsonName) ? propertyName : jsonName;
        }
    }
}