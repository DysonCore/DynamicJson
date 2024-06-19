using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal sealed class TypeConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _typeMap = new ();
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not Type type)
            {
                return;
            }
            
            string typeName = type.AssemblyQualifiedName;
            writer.WriteValue(typeName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            string typeName = token.ToObject<string>();

            if (_typeMap.TryGetValue(typeName, out Type type))
            {
                //do not resolve the type if it is already cached
                return type;
            }
            
            type = Type.GetType(typeName);
            _typeMap[typeName] = type;
            
            return type;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Type);
        }
    }
}