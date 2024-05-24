using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    public class PolymorphicCache
    {
        [JsonIgnore]
        internal Dictionary<Type, TypifyingPropertyData> Data { get; private set; }
        
        [JsonProperty("data")] 
        private List<KeyValuePair<Type, TypifyingPropertyData>> _dataSerializationBuffer;

        internal PolymorphicCache(Dictionary<Type, TypifyingPropertyData> data)
        {
            Data = data;
        }
        
        [JsonConstructor]
        private PolymorphicCache() { }
        
        [OnSerializing]
        internal void OnBeforeSerialization(StreamingContext context)
        {
            _dataSerializationBuffer = Data.ToList();
        }

        [OnDeserialized]
        internal void OnAfterDeserialization(StreamingContext context)
        {
            foreach (var pair in _dataSerializationBuffer)
            {
                //[OnDeserialized] attribute is called only on the root object. So it should be called manually in this case.
                pair.Value.OnAfterDeserialization(context);
            }
            
            Data = _dataSerializationBuffer.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}