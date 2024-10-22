using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal static class PolymorphicCacheProvider
    {
        private static PolymorphicCache _data;
        
        private static JsonSerializerSettings SerializerSettings => CacheSerializerSettingsProvider.Settings;
        
        internal static PolymorphicCache GetData()
        {
            if (_data is not null)
            {
                return _data;
            }

            string filePath = Path.Combine(PolymorphicCacheConstants.CacheDirectoryName, PolymorphicCacheConstants.FileName);
            
            TextAsset jsonTextAsset = Resources.Load<TextAsset>(filePath);

            if (jsonTextAsset == null)
            {
                throw new FileNotFoundException($"[{nameof(PolymorphicCacheProvider)}.{nameof(GetData)}] Critical Error! Cache file was not found!");
            }
            
            _data = JsonConvert.DeserializeObject<PolymorphicCache>(jsonTextAsset.text, SerializerSettings);
            Resources.UnloadAsset(jsonTextAsset);
            
            return _data;
        }
    }
}