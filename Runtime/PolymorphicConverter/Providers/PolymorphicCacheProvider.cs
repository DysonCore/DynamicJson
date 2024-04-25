using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DysonCore.DynamicJson.PolymorphicConverter
{
    internal static class PolymorphicCacheProvider
    {
        private static Dictionary<TypeLazyReference, TypifyingPropertyData> _data;
        
        internal static Dictionary<TypeLazyReference, TypifyingPropertyData> GetData()
        {
            if (_data != null)
            {
                return _data;
            }

            string filePath = Path.Combine(PolymorphicCacheConstants.CacheDirectoryName, PolymorphicCacheConstants.FileName);
            
            List<JsonConverter> converters = new List<JsonConverter>
            {
                new DictionaryAsArrayJsonConverter(),
                new TypeLazyReferenceConverter()
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = converters
            };

            TextAsset jsonTextFile = Resources.Load<TextAsset>(filePath);
            _data = JsonConvert.DeserializeObject<Dictionary<TypeLazyReference, TypifyingPropertyData>>(jsonTextFile.text, settings);
            
            return _data;
        }
    }
}