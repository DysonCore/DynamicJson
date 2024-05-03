using System;
using System.Collections.Generic;
using System.IO;
using DysonCore.DynamicJson.SafeStringEnumParser;
using Newtonsoft.Json;
using UnityEngine;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal static class PolymorphicCacheProvider
    {
        private static Dictionary<Type, TypifyingPropertyData> _data;
        
        internal static Dictionary<Type, TypifyingPropertyData> GetData()
        {
            if (_data != null)
            {
                return _data;
            }

            string filePath = Path.Combine(PolymorphicCacheConstants.CacheDirectoryName, PolymorphicCacheConstants.FileName);
            
            List<JsonConverter> converters = new List<JsonConverter>
            {
                new DictionaryAsArrayJsonConverter(),
                new TypeConverter(),
                new SafeStringEnumConverter()
            };

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = converters
            };

            TextAsset jsonTextFile = Resources.Load<TextAsset>(filePath);
            _data = JsonConvert.DeserializeObject<Dictionary<Type, TypifyingPropertyData>>(jsonTextFile.text, settings);
            
            return _data;
        }
    }
}