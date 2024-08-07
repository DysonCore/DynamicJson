using System;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    /// <summary>
    /// Represents metadata for typified properties. 
    /// Contains information about property type, name, JSON name.
    /// </summary>
    internal sealed class TypifiedPropertyData : PropertyData
    {
        internal TypifiedPropertyData(Type propertyType, string propertyName, string jsonName) 
            : base(propertyType, propertyName, jsonName) { }

        [JsonConstructor]
        private TypifiedPropertyData() { }
    }
}