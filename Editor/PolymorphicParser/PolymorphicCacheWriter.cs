using System.Collections.Generic;
using System.IO;
using DysonCore.DynamicJson.PolymorphicParser;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DysonCore.DynamicJson.Editor.PolymorphicParser
{
    [InitializeOnLoad]
    internal static class PolymorphicCacheWriter
    {
        private static JsonSerializerSettings _settings;

        private static JsonSerializerSettings SerializerSettings => _settings ??= CreateSerializerSettings();

        static PolymorphicCacheWriter()
        {
            object data = PolymorphicCacheBuilder.GetData();
            CreateCache(data);
        }

        private static void CreateCache(object data)
        {
            string directoryLocalPath = Path.Combine(PolymorphicCacheConstants.ResourcesDirectoryName, PolymorphicCacheConstants.CacheDirectoryName);
            string directoryGlobalPath = Path.Combine(Application.dataPath, directoryLocalPath);
            string globalFilePath = Path.Combine(directoryGlobalPath, PolymorphicCacheConstants.FullFileName);
            
            Directory.CreateDirectory(directoryGlobalPath);
            
            using StreamWriter writer = new StreamWriter(globalFilePath);
            using JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            
            serializer.Serialize(jsonWriter, data);
        }
        
        private static JsonSerializerSettings CreateSerializerSettings()
        {
            List<JsonConverter> converters = new List<JsonConverter>
            {
                new DictionaryAsArrayJsonConverter(),
                new TypeLazyReferenceConverter()
            };
            
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = converters
            };

            return settings;
        }
    }
}