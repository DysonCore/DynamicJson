using System.Collections.Generic;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    /// <summary>
    /// Represents metadata for properties marked with the <see cref="TypifyingPropertyAttribute"/>. 
    /// Contains information about property type, name, JSON name, and the mapping between values and their respective types, as well as any <see cref="TypifyingPropertyAttribute"/>s associated with this <see cref="TypifiedPropertyAttribute"/>.
    /// </summary>
    internal class TypifyingPropertyData : PropertyData
    {
        [JsonProperty("valuesData")]
        internal Dictionary<object, TypeLazyReference> ValuesData { get; }
        [JsonProperty("typifiedProperties")]
        internal List<TypifiedPropertyData> TypifiedProperties { get; }

        internal TypifyingPropertyData(TypeLazyReference propertyType, string propertyName, string jsonName, TypifyingPropertyAttribute baseTypifyingAttribute) : base(propertyType, propertyName, jsonName)
        {
            ValuesData = new Dictionary<object, TypeLazyReference>();
            TypifiedProperties = new List<TypifiedPropertyData>();
        }
    }
}