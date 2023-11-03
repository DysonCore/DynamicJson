using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.PolymorphicJson
{
    /// <summary>
    /// Provides custom JSON deserialization for objects marked with <see cref="TypifyingPropertyAttribute"/>.
    /// </summary>
    public sealed class PolymorphicJsonConverter: JsonConverter
    {
        private static readonly Dictionary<Type, PropertyData> BaseToPropertyData = new ();
        private static bool _initialized;

        private JsonSerializer _defaultSerializer;
        private JsonSerializer GetDefaultSerializer(JsonSerializer serializer) => _defaultSerializer ??= CreateDefaultSerializer(serializer);

        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicJsonConverter"/> class.
        /// </summary>
        public PolymorphicJsonConverter()
        {
            Init();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PolymorphicJsonConverter"/> class.
        /// Takes a list of assemblies to be used for initialization.
        /// </summary>
        /// <param name="assembliesToUse">List of assemblies to use for initialization.</param>
        public PolymorphicJsonConverter(List<Assembly> assembliesToUse)
        {
            Init(assembliesToUse);
        }

        private void Init(List<Assembly> assembliesToUse = null)
        {
            if (assembliesToUse is { Count: > 0 })
            {
                BaseToPropertyData.Clear();
                InitializeConverter(assembliesToUse);
            }
            else
            {
                if (!_initialized)
                {
                    InitializeConverter();
                }
            }

            _initialized = true;
        }

        /// <summary>
        /// Initializes the converter by scanning the assemblies for types with the <see cref="TypifyingPropertyAttribute"/> 
        /// and populates the BaseToPropertyData dictionary.
        /// </summary>
        /// <param name="assembliesToUse">List of assemblies to scan, if any. If null, the executing assembly and assemblies referencing current assembly are used.</param>
        private static void InitializeConverter(List<Assembly> assembliesToUse = null)
        {
            List<(Type abstractType, PropertyData propertyData)> abstractDefiningData = new ();
            List<Assembly> assemblies = assembliesToUse ?? Assembly.GetExecutingAssembly().GetReferencingAssemblies();
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type classType in assembly.GetTypes())
                {
                    foreach (PropertyInfo propertyInfo in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        TypifyingPropertyAttribute typifyingAttribute = propertyInfo.GetCustomAttribute<TypifyingPropertyAttribute>();
                        
                        if (typifyingAttribute == null)
                        {
                            continue;
                        }

                        Type propertyType = propertyInfo.PropertyType;
                        JsonPropertyAttribute jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();

                        Type baseClass = classType.GetDeclaringClass(propertyInfo.Name);
                        
                        if (!BaseToPropertyData.TryGetValue(baseClass, out PropertyData propertyData))
                        {
                            propertyData = new PropertyData(propertyType, propertyInfo.Name, jsonProperty?.PropertyName);
                            BaseToPropertyData[baseClass] = propertyData;
                        }
                        
                        if (classType.IsAbstract)
                        {
                            if (classType != baseClass)
                            {
                                abstractDefiningData.Add((classType, propertyData));
                            }
                            continue;
                        }

                        object classInstance = Activator.CreateInstance(classType, true);
                        object propertyValue = propertyInfo.GetValue(classInstance);
                        
                        propertyData.ValuesData[propertyValue] = classType;
                    }
                }
            }
            
            foreach (var data in abstractDefiningData)
            {
                if(!BaseToPropertyData.TryGetValue(data.abstractType, out PropertyData propertyData))
                {
                    continue;
                }
                
                Type implementingClass = propertyData.ValuesData.Values.FirstOrDefault(type => !type.IsAbstract);
                PropertyInfo propertyInfo = implementingClass?.GetProperty(data.propertyData.PropertyName);

                if (propertyInfo is null)
                {
                    continue;
                }

                object classObject = Activator.CreateInstance(implementingClass, true);
                object value = propertyInfo.GetValue(classObject);
                
                data.propertyData.ValuesData[value] = data.abstractType;
            }
        }

        /// <summary>
        /// Deserializes a JSON object based on the defined typifying property.
        /// </summary>
        /// <returns>The deserialized object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (!BaseToPropertyData.TryGetValue(objectType, out PropertyData propertyData))
            {
                return token.ToObject(objectType, GetDefaultSerializer(serializer));
            }
            
            JToken typeToken = token.SelectToken(propertyData.JsonName);
            
            if (typeToken is null)
            {
                throw new JsonSerializationException($"[{nameof(PolymorphicJsonConverter)}.{nameof(ReadJson)}] Can't find typified json token for type {objectType.FullName}. Used property name {propertyData.JsonName}.\nJObject - {token}");
            }
            
            object value = typeToken.ToObject(propertyData.PropertyType);

            if (value is null || !propertyData.ValuesData.TryGetValue(value, out Type implementer))
            {
                throw new JsonSerializationException($"[{nameof(PolymorphicJsonConverter)}.{nameof(ReadJson)}] Can't parse typifying token or find concrete class. Typifying token - {typeToken}. Object type - {objectType.FullName}. Used type - {propertyData.PropertyType.FullName}");
            }

            JsonSerializer jsonSerializer = objectType.IsAbstract ? serializer : GetDefaultSerializer(serializer);
            
            return token.ToObject(implementer, jsonSerializer);
        }

        /// <summary>
        /// Determines if the given type can be converted using this converter.
        /// </summary>
        /// <param name="objectType">Type of the object to check.</param>
        /// <returns>True if the type can be converted; otherwise, false.</returns>
        public override bool CanConvert(Type objectType)
        {
            return BaseToPropertyData.ContainsKey(objectType);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){}

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <summary>
        /// Creates a default JsonSerializer instance without the PolymorphicJsonConverter.
        /// </summary>
        /// <param name="currentSerializer">The current serializer being used.</param>
        /// <returns>A new JsonSerializer instance.</returns>
        private JsonSerializer CreateDefaultSerializer(JsonSerializer currentSerializer)
        {
            JsonSerializer defaultSerializer = new JsonSerializer();
            
            List<JsonConverter> defaultSettings = currentSerializer.Converters.ToList();
            defaultSettings.RemoveAll(converter => converter is PolymorphicJsonConverter);
            
            foreach (JsonConverter converter in defaultSettings)
            {
                defaultSerializer.Converters.Add(converter);
            }

            defaultSerializer.ContractResolver = new IgnoreConvertersContractResolver(typeof(PolymorphicJsonConverter));

            return defaultSerializer;
        }
    }
}