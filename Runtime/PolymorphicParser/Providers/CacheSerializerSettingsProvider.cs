using System.Collections.Generic;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal static class CacheSerializerSettingsProvider
    {
        private static JsonSerializerSettings _settings;

        internal static JsonSerializerSettings Settings => _settings ??= CreateSerializerSettings();
        
        private static JsonSerializerSettings CreateSerializerSettings()
        {
            List<JsonConverter> converters = new List<JsonConverter>
            {
                new TypeConverter(),
                new TypifyingPropertyConverter()
            };
            
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = converters
            };

            return settings;
        }
    }
}