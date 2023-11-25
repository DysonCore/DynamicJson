using System;

namespace DysonCore.PolymorphicJson.Models
{
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