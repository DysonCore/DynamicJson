using System;
using System.Collections.Generic;
using System.Reflection;
using DysonCore.PolymorphicJson.Attributes;
using DysonCore.PolymorphicJson.Enums;
using DysonCore.PolymorphicJson.Models;
using DysonCore.PolymorphicJson.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.PolymorphicJson.Converters
{
    /// <summary>
    /// Provides custom JSON deserialization for objects marked with <see cref="TypifyingPropertyAttribute"/> and <see cref="TypifiedPropertyAttribute"/>.
    /// </summary>
    public sealed class PolymorphicJsonConverter: JsonConverter
    {
        private Dictionary<Type, TypifyingPropertyData> BaseToPropertyData => PropertyDataProvider.BaseToPropertyData;
        private readonly List<Type> _typesToIgnore = new ();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicJsonConverter"/> class.
        /// Optionally takes an array of assemblies to be used for initialization of <see cref="PropertyDataProvider"/>.
        /// </summary>
        /// <param name="assembliesToUse">Optional array of assemblies to use for initialization.</param>
        public PolymorphicJsonConverter(params Assembly[] assembliesToUse)
        {
            PropertyDataProvider.Initialize(assembliesToUse);
        }

        /// <summary>
        /// Deserializes a JSON object based on the defined <see cref="TypifyingPropertyAttribute"/>.
        /// </summary>
        /// <returns>The deserialized object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            _typesToIgnore.Clear();
            JToken token = JToken.Load(reader);

            if (!BaseToPropertyData.TryGetValue(objectType, out TypifyingPropertyData propertyData) || propertyData.ValuesData.Count <= 0)
            {
                _typesToIgnore.Add(objectType);
                return token.ToObject(objectType, serializer);
            }
            
            JToken typifyingToken = token.SelectToken(propertyData.JsonName);
            object value = typifyingToken?.ToObject(propertyData.PropertyType);

            if (value is null || !propertyData.ValuesData.TryGetValue(value, out Type implementer))
            {
                if (propertyData.TypifiedProperties.Count <= 0)
                {
                    switch (propertyData.TypifyingAttribute.UnknownTypeHandling)
                    {
                        case UnknownTypeHandling.ReturnNull: return null;
                        case UnknownTypeHandling.ThrowError:
                        default: throw new JsonReaderException($"[{nameof(PolymorphicJsonConverter)}.{nameof(ReadJson)}] Can't parse typifying token or find concrete class. Typifying token - {typifyingToken}. Object type - {objectType.FullName}. Used type - {propertyData.PropertyType.FullName}");
                    }
                }

                implementer = objectType;
            }

            token = TypifyTokenMembers(token, typifyingToken, propertyData);

            if (objectType == implementer || !objectType.IsAbstract)
            {
                _typesToIgnore.Add(implementer);
            }
            
            return token.ToObject(implementer, serializer);
        }

        /// <summary>
        /// Adds typifying <see cref="JToken"/> to the current token members which are marked with <see cref="TypifiedPropertyAttribute"/>.
        /// </summary>
        /// <returns><see cref="JToken"/> with typifying token added to the corresponding members.</returns>
        private JToken TypifyTokenMembers(JToken currentToken, JToken typifyingToken, TypifyingPropertyData propertyData)
        {
            if (propertyData.TypifiedProperties.Count <= 0 || currentToken is not JObject currentJObject)
            {
                return currentToken;
            }

            foreach (TypifiedPropertyData typifiedProperty in propertyData.TypifiedProperties)
            {
                if (!currentJObject.TryGetValue(typifiedProperty.JsonName, out JToken valueToken))
                {
                    return null;
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
            return BaseToPropertyData.ContainsKey(objectType) && !_typesToIgnore.Contains(objectType);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){}

        /// <inheritdoc />
        public override bool CanWrite => false;
    }
}