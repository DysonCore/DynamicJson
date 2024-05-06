using System;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal class TypifyingPropertyConverter : JsonConverter
    {
        private static Type TypeToConvert => typeof(TypifyingPropertyData);
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == TypeToConvert;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

        /// <inheritdoc />
        public override bool CanWrite => false;
    }
}