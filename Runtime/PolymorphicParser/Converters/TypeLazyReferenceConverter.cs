using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal sealed class TypeLazyReferenceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not TypeLazyReference lazyReference)
            {
                return;
            }
            
            string serializedValue = lazyReference.TypeName;
            writer.WriteValue(serializedValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            string typeName = token.ToObject<string>();
            TypeLazyReference lazyReference = new TypeLazyReference(typeName);
            return lazyReference;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TypeLazyReference);
        }
    }
}