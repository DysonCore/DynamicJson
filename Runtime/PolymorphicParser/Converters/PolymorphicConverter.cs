using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    /// <summary>
    /// Provides custom JSON deserialization for objects marked with <see cref="TypifiedPropertyAttribute"/> and <see cref="TypifyingPropertyAttribute"/>.
    /// </summary>
    public sealed class PolymorphicConverter : JsonConverter
    {
        private readonly PolymorphicCache _polymorphicCache;
        private readonly ThreadLocal<List<Type>> _typesToIgnore = new (() => new List<Type>());
        
        private Dictionary<Type, TypifyingPropertyData> BaseToPropertyData => _polymorphicCache.Data;

        private UnknownTypeHandling UnknownTypeHandling { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicConverter"/> class.
        /// Optionally takes an array of assemblies to be used for initialization of <see cref="PolymorphicCacheProvider"/>.
        /// </summary>
        /// <param name="unknownTypeHandling">Specifies how the converter treats unknown type cases. <see cref="UnknownTypeHandling.ThrowError"/> is used by default.</param>
        public PolymorphicConverter(UnknownTypeHandling unknownTypeHandling = UnknownTypeHandling.ThrowError)
        {
            UnknownTypeHandling = unknownTypeHandling;
            _polymorphicCache = PolymorphicCacheProvider.GetData();
        }

        /// <summary>
        /// Deserializes a JSON object based on the defined <see cref="TypifyingPropertyAttribute"/>.
        /// </summary>
        /// <returns>The deserialized object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            _typesToIgnore.Value.Clear();
            JToken token = JToken.Load(reader);
            object toReturn;

            if (!BaseToPropertyData.TryGetValue(objectType, out TypifyingPropertyData propertyData))
            {
                _typesToIgnore.Value.Add(objectType);
                toReturn = token.ToObject(objectType, serializer);
                _typesToIgnore.Value.Clear();
                return toReturn;
            }
            
            JToken typifyingToken = token.SelectToken(propertyData.JsonName);
            object value = typifyingToken?.ToObject(propertyData.PropertyType, serializer);

            if (value is null || !propertyData.ValuesData.TryGetValue(value, out Type implementer))
            {
                if (propertyData.TypifiedProperties.IsNullOrEmpty())
                {
                    switch (UnknownTypeHandling)
                    {
                        case UnknownTypeHandling.ReturnNull: return null;
                        case UnknownTypeHandling.ThrowError:
                        default: throw new JsonReaderException($"[{nameof(PolymorphicConverter)}.{nameof(ReadJson)}] Can't parse typifying token or find concrete class. Typifying token - {typifyingToken}. Object type - {objectType.FullName}. Used type - {propertyData.PropertyType.FullName}");
                    }
                }

                implementer = objectType;
            }

            token = TypifyTokenMembers(token, typifyingToken, propertyData);

            if (objectType == implementer || !objectType.IsAbstract)
            {
                _typesToIgnore.Value.Add(implementer);
            }
            
            toReturn = token.ToObject(implementer, serializer);
            _typesToIgnore.Value.Clear();
            return toReturn;
        }

        /// <summary>
        /// Adds typifying <see cref="JToken"/> to the current token members which are marked with <see cref="TypifiedPropertyAttribute"/>.
        /// </summary>
        /// <returns><see cref="JToken"/> with typifying token added to the corresponding members.</returns>
        private JToken TypifyTokenMembers(JToken currentToken, JToken typifyingToken, TypifyingPropertyData propertyData)
        {
            if (propertyData.TypifiedProperties.IsNullOrEmpty() || currentToken is not JObject currentJObject)
            {
                return currentToken;
            }

            foreach (TypifiedPropertyData typifiedProperty in propertyData.TypifiedProperties)
            {
                if (!currentJObject.TryGetValue(typifiedProperty.JsonName, out JToken valueToken))
                {
                    continue;
                }
                
                switch (valueToken)
                {
                    case JObject jObject:
                        jObject.TryAdd(propertyData.JsonName, typifyingToken);
                        break;
                    case JArray jArray:
                        foreach (JToken token in jArray)
                        {
                            if (token is not JObject jObject)
                            {
                                continue;
                            }
                            jObject.TryAdd(propertyData.JsonName, typifyingToken);
                        }
                        break;
                }
            }
            
            return currentJObject;
        }

        /// <summary>
        /// Determines if the given type can be converted using this converter.
        /// </summary>
        /// <param name="objectType">Type of the object to check.</param>
        /// <returns>True if the type can be converted; otherwise, false.</returns>
        public override bool CanConvert(Type objectType)
        {
            return BaseToPropertyData.ContainsKey(objectType) && !_typesToIgnore.Value.Contains(objectType);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

        /// <inheritdoc />
        public override bool CanWrite => false;
    }
}