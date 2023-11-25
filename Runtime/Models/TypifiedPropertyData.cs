using System;

namespace DysonCore.PolymorphicJson.Models
{
    /// <summary>
    /// Represents metadata for typified properties. 
    /// Contains information about property type, name, JSON name.
    /// </summary>
    internal class TypifiedPropertyData : PropertyData
    {
        internal TypifiedPropertyData(Type propertyType, string propertyName, string jsonName) : base(propertyType, propertyName, jsonName)
        {
        }
    }
}