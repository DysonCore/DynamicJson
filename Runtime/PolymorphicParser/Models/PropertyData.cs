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
        [JsonIgnore]
        public string PropertyName { get; private set; }
        
        [JsonProperty("propertyType")]
        public Type PropertyType { get; protected set; } // Any Property for deserialization in the base class needs to have protected setter (not private),
                                                         // since newtonsoft cant set data into fields to which upper class has no access.  
        [JsonProperty("name")]
        public string JsonName { get; protected set; }

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