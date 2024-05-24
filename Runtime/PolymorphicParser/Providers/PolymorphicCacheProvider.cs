using System.Collections.Generic;
using System.IO;
using DysonCore.DynamicJson.SafeStringEnumParser;
using Newtonsoft.Json;
using UnityEngine;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal static class PolymorphicCacheProvider
    {
        private static PolymorphicCache _data;
        
        internal static PolymorphicCache GetData()
        {
            if (_data != null)
            {
                return _data;
            }

            string filePath = Path.Combine(PolymorphicCacheConstants.CacheDirectoryName, PolymorphicCacheConstants.FileName);
            
            List<JsonConverter> converters = new List<JsonConverter>
            {
                new TypeConverter(),
                new SafeStringEnumConverter(),
                new TypifyingPropertyConverter()
            };

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = converters
            };

            TextAsset jsonTextAsset = Resources.Load<TextAsset>(filePath);
            _data = JsonConvert.DeserializeObject<PolymorphicCache>(jsonTextAsset.text, settings);
            
            return _data;
        }
    }
}