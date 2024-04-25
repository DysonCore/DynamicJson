using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicConverter
{
    /// <summary>
    /// Base class representing metadata for properties used by polymorphic converter. 
    /// Contains information about property type, name, JSON name.
    /// </summary>
    internal abstract class PropertyData
    {
        [JsonProperty("propertyType")]
        internal TypeLazyReference PropertyType { get; }
        
        [JsonProperty("name")]
        internal string PropertyName { get; }
        
        [JsonProperty("jsonName")]
        internal string JsonName { get; }

        protected PropertyData(TypeLazyReference propertyType, string propertyName, string jsonName)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            JsonName = string.IsNullOrWhiteSpace(jsonName) ? propertyName : jsonName;
        }
    }
}