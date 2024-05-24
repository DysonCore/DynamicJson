using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal sealed class TypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not Type type)
            {
                return;
            }
            
            string serializedValue = type.AssemblyQualifiedName;
            writer.WriteValue(serializedValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            string typeName = token.ToObject<string>();
            Type type = Type.GetType(typeName);
            return type;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Type);
        }
    }
}