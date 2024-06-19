using System.IO;
using DysonCore.DynamicJson.PolymorphicParser;
using Newtonsoft.Json;
using UnityEngine;

namespace DysonCore.DynamicJson.Editor.PolymorphicParser
{
    internal static class CacheWriter
    {
        private static JsonSerializerSettings SerializerSettings => CacheSerializerSettingsProvider.Settings;

        internal static void CreateCache()
        {
            object cacheData = CacheBuilder.GetData();
            
            string directoryLocalPath = Path.Combine(PolymorphicCacheConstants.ResourcesDirectoryName, PolymorphicCacheConstants.CacheDirectoryName);
            string directoryGlobalPath = Path.Combine(Application.dataPath, directoryLocalPath);
            
            string filePath = Path.Combine(directoryGlobalPath, PolymorphicCacheConstants.FullFileName);
    
            Directory.CreateDirectory(directoryGlobalPath); // Does not require to manually check if directory exists beforehand
                                                            // since existing directory will be skipped. 
            using StreamWriter writer = new StreamWriter(filePath);
            using JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            
            serializer.Serialize(jsonWriter, cacheData);
        }
    }
}