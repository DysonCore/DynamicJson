using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.PolymorphicConverter
{
    /// <summary>
    /// Provides custom JSON deserialization for objects marked with <see cref="TypifiedPropertyAttribute"/> and <see cref="TypifyingPropertyAttribute"/>.
    /// </summary>
    public sealed class PolymorphicConverter : JsonConverter
    {
        private Dictionary<Type, TypifyingPropertyData> BaseToPropertyData => PropertyDataProvider.BaseToPropertyData;
        private readonly List<Type> _typesToIgnore = new ();

        private UnknownTypeHandling UnknownTypeHandling { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicConverter"/> class.
        /// Optionally takes an array of assemblies to be used for initialization of <see cref="PropertyDataProvider"/>.
        /// </summary>
        /// <param name="unknownTypeHandling">Specifies how the converter treats unknown type cases. <see cref="UnknownTypeHandling.ThrowError"/> is used by default.</param>
        /// <param name="assembliesToUse">Optional array of assemblies to use for initialization.</param>
        public PolymorphicConverter(UnknownTypeHandling unknownTypeHandling = UnknownTypeHandling.ThrowError, params Assembly[] assembliesToUse)
        {
            UnknownTypeHandling = unknownTypeHandling;
            PropertyDataProvider.Initialize(assembliesToUse);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicConverter"/> class.
        /// Optionally takes an array of assemblies to be used for initialization of <see cref="PropertyDataProvider"/>.
        /// </summary>
        /// <param name="unknownTypeHandling">Specifies how the converter treats unknown type cases. <see cref="UnknownTypeHandling.ThrowError"/> is used by default.</param>
        public PolymorphicConverter(UnknownTypeHandling unknownTypeHandling = UnknownTypeHandling.ThrowError)
        {
            UnknownTypeHandling = unknownTypeHandling;
            PropertyDataProvider.Initialize();
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
            object value = typifyingToken?.ToObject(propertyData.PropertyType, serializer);

            if (value is null || !propertyData.ValuesData.TryGetValue(value, out Type implementer))
            {
                if (propertyData.TypifiedProperties.Count <= 0)
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
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

        /// <inheritdoc />
        public override bool CanWrite => false;
    }
}