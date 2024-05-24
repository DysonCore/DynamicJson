using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    /// <summary>
    /// Represents metadata for properties marked with the <see cref="TypifyingPropertyAttribute"/>. 
    /// Contains information about property type, name, JSON name, and the mapping between values and their respective types, as well as any <see cref="TypifyingPropertyAttribute"/>s associated with this <see cref="TypifiedPropertyAttribute"/>.
    /// </summary>
    internal class TypifyingPropertyData : PropertyData
    {
        [JsonProperty("typifiedProperties")]
        internal List<TypifiedPropertyData> TypifiedProperties { get; private set; }
        
        [JsonIgnore] 
        internal Dictionary<object, Type> ValuesData { get; private set; }

        [JsonProperty("valuesData")] 
        internal List<KeyValuePair<object, Type>> ValuesSerializationBuffer { get; private set; }

        internal TypifyingPropertyData(Type propertyType, string propertyName, string jsonName) 
            : base(propertyType, propertyName, jsonName)
        {
            ValuesData = new Dictionary<object, Type>();
            TypifiedProperties = new List<TypifiedPropertyData>();
        }

        [JsonConstructor]
        private TypifyingPropertyData() { }
        
        [OnSerializing]
        internal void OnBeforeSerialization(StreamingContext context)
        {
            ValuesSerializationBuffer = ValuesData.ToList();
            
        }

        [OnDeserialized]
        internal void OnAfterDeserialization(StreamingContext context)
        {
            ValuesData = ValuesSerializationBuffer.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}