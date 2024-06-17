using System.IO;
using DysonCore.DynamicJson.PolymorphicParser;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.Editor.PolymorphicParser
{
    internal static class CacheWriter
    {
        private static object _cacheData;

        private static object CacheData => _cacheData ??= CacheBuilder.GetData();
        private static JsonSerializerSettings SerializerSettings => CacheSerializerSettingsProvider.Settings;

        internal static void CreateCache(string directoryPath)
        {
            string filePath = Path.Combine(directoryPath, PolymorphicCacheConstants.CacheFileName);
    
            Directory.CreateDirectory(directoryPath); // Does not require to manually check if directory exists beforehand
                                                      // since existing directory will be skipped. 
            using StreamWriter writer = new StreamWriter(filePath);
            using JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            
            serializer.Serialize(jsonWriter, CacheData);
        }
    }
}