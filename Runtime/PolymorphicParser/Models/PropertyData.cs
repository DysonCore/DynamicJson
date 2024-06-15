using System;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    /// <summary>
    /// Base class representing metadata for properties used by polymorphic converter. 
    /// Contains information about property type, name, JSON name.
    /// </summary>
    internal abstract class PropertyData
    {
        [JsonProperty("propertyType")]
        public Type PropertyType { get; protected set; }
        
        [JsonProperty("name")]
        public string JsonName { get; protected set; }
        
        [JsonIgnore]
        public string PropertyName { get; protected set; }

        protected internal PropertyData(Type propertyType, string propertyName, string jsonName)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            JsonName = string.IsNullOrWhiteSpace(jsonName) ? propertyName : jsonName;
        }

        [JsonConstructor]
        protected PropertyData() { }
    }
}