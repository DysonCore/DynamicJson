using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal class TypifyingPropertyConverter : JsonConverter
    {
        private static Type TypeToConvert => typeof(TypifyingPropertyData);
        private static Type PairGenericType => typeof(KeyValuePair<,>);
        private static Type ListGenericType => typeof(List<>);
        private PropertyInfo[] Properties => _properties ??= TypeToConvert.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

        private PropertyInfo[] _properties;
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            TypifyingPropertyData data = existingValue as TypifyingPropertyData ?? (TypifyingPropertyData)Activator.CreateInstance(TypeToConvert, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, new object[] {}, null);
            JToken valuesBufferToken = null;
            PropertyInfo bufferPropertyInfo = null;
            
            foreach (PropertyInfo propertyInfo in Properties)
            {
                JsonPropertyAttribute jsonAttribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
                string propertyName = jsonAttribute?.PropertyName ?? propertyInfo.Name;
                
                if (!jObject.TryGetValue(propertyName, out JToken token))
                {
                    continue;
                }

                if (propertyInfo.Name == nameof(data.ValuesSerializationBuffer))
                {
                    valuesBufferToken = token;
                    bufferPropertyInfo = propertyInfo;
                    continue;
                }
                
                object value = token.ToObject(propertyInfo.PropertyType, serializer);
                propertyInfo.SetValue(data, value);
            }

            Type bufferType = ConstructBufferType(data.PropertyType);
            object bufferValue = valuesBufferToken?.ToObject(bufferType, serializer);

            List<KeyValuePair<object, Type>> buffer = PostProcessBufferValue(bufferValue);
            bufferPropertyInfo?.SetValue(data, buffer);
            
            return data;
        }

        private Type ConstructBufferType(Type keyType)
        {
            Type pairType = PairGenericType.MakeGenericType(keyType, typeof(Type));
            Type listType = ListGenericType.MakeGenericType(pairType);

            return listType;
        }

        private List<KeyValuePair<object, Type>> PostProcessBufferValue(object input)
        {
            Type inputType = input.GetType();

            if (!inputType.IsGenericType || inputType.GetGenericTypeDefinition() != ListGenericType || input is not IEnumerable list)
            {
                throw new Exception($"[{nameof(TypifyingPropertyConverter)}.{nameof(PostProcessBufferValue)}] Provided object of {nameof(Type)} {inputType.Name} is not compatible for post processing");
            }
            
            List<KeyValuePair<object, Type>> toReturn = new List<KeyValuePair<object, Type>>();
            
            foreach (object item in list)
            {
                Type itemType = item.GetType();
                if (!itemType.IsGenericType || itemType.GetGenericTypeDefinition() != PairGenericType)
                {
                    continue;
                }

                object key = itemType.GetProperty(TypifyingPropertyConstants.KeyName)?.GetValue(item);
                object value = itemType.GetProperty(TypifyingPropertyConstants.ValueName)?.GetValue(item);
                
                toReturn.Add(new KeyValuePair<object, Type>(key, (Type)value));
            }

            return toReturn;
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