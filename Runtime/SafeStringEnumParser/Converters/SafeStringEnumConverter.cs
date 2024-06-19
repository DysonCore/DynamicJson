using System;
using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DysonCore.DynamicJson.SafeStringEnumParser
{
    public sealed class SafeStringEnumConverter : StringEnumConverter
    {
        private static readonly ConcurrentDictionary<Type, object> DefaultsMap = new ();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeStringEnumConverter"/> class.
        /// Mirrors <see cref="StringEnumConverter"/>'s constructor.
        /// </summary>
        public SafeStringEnumConverter() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeStringEnumConverter"/> class with a flag indicating whether enum values are written as camel case.
        /// Mirrors <see cref="StringEnumConverter"/>'s constructor.
        /// </summary>
        /// <param name="camelCaseText"><c>true</c> to write enum values as camel case; <c>false</c> to write them as they are defined.</param>
        [Obsolete("StringEnumConverter(bool) base constructor is obsolete. Create a converter with SafeStringEnumConverter(NamingStrategy, bool) instead.")]
        public SafeStringEnumConverter(bool camelCaseText) : base(camelCaseText) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeStringEnumConverter"/> class with a specific naming strategy type and parameters.
        /// Mirrors <see cref="StringEnumConverter"/>'s constructor.
        /// </summary>
        /// <param name="namingStrategyType">The type of the naming strategy to apply.</param>
        /// <param name="namingStrategyParameters">An array of parameters for the naming strategy.</param>
        public SafeStringEnumConverter(Type namingStrategyType, object[] namingStrategyParameters) : base(namingStrategyType, namingStrategyParameters) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeStringEnumConverter"/> class with a specific naming strategy and a flag indicating whether integer values are allowed.
        /// Mirrors <see cref="StringEnumConverter"/>'s constructor.
        /// </summary>
        /// <param name="namingStrategy">The naming strategy to apply.</param>
        /// <param name="allowIntegerValues">true to allow undefined enum values; false to throw an exception when encountering undefined values.</param>
        public SafeStringEnumConverter(NamingStrategy namingStrategy, bool allowIntegerValues = true) : base(namingStrategy, allowIntegerValues) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeStringEnumConverter"/> class with a specific naming strategy type.
        /// Mirrors <see cref="StringEnumConverter"/>'s constructor.
        /// </summary>
        /// <param name="namingStrategyType">The type of the naming strategy to apply.</param>
        public SafeStringEnumConverter(Type namingStrategyType) : base(namingStrategyType) { }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception)
            {
                object defaultValue = GetDefaultValue(objectType);
                
                if (defaultValue != null)
                {
                    return defaultValue;
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the default value of the enum type specified using <see cref="DefaultEnumValueAttribute"/>.
        /// </summary>
        /// <param name="type">Type of the enum.</param>
        /// <returns>The default enum value, or null if none is found.</returns>
        private object GetDefaultValue(Type type)
        {
            Type actualType = Nullable.GetUnderlyingType(type) ?? type; //is used to support nullable enums

            if(DefaultsMap.TryGetValue(actualType, out object value)) //check cache for resolved defaults 
            {
                return value;
            }
            
            foreach (FieldInfo field in actualType.GetFields())
            {
                if (field.GetCustomAttribute<DefaultEnumValueAttribute>() == null)
                {
                    continue;
                }
                
                value = Enum.Parse(actualType, field.Name);
                DefaultsMap[actualType] = value;
                break;
            }
            
            return value;
        }
    }
}