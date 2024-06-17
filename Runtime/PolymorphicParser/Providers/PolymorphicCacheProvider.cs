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

            string directoryPath = Path.Combine(Application.streamingAssetsPath, PolymorphicCacheConstants.CacheDirectoryName);
            string filePath = Path.Combine(directoryPath, PolymorphicCacheConstants.CacheFileName);

            if (File.Exists(filePath) is false)
            {
                throw new FileNotFoundException($"[{nameof(PolymorphicCacheProvider)}.{nameof(GetData)}] Cache file is missing! Report to package developer if occurs.");
            }
            
            using StreamReader reader = new StreamReader(filePath);
            string jsonData = reader.ReadToEnd();
            
            _data = JsonConvert.DeserializeObject<PolymorphicCache>(jsonData, SerializerSettings);
            
            return _data;
        }
    }
}