using System.IO;
using DysonCore.DynamicJson.PolymorphicParser;
using UnityEditor;
using UnityEngine;

namespace DysonCore.DynamicJson.Editor.PolymorphicParser
{
    [InitializeOnLoad]
    internal static class ScriptReloadProcessor
    {

        static ScriptReloadProcessor()
        {
            OnScriptsReloaded();
        }

        private static void OnScriptsReloaded()
        {
            string directoryPath = Path.Combine(Application.streamingAssetsPath, PolymorphicCacheConstants.CacheDirectoryName);
            
            CacheWriter.CreateCache(directoryPath);
        }
        
    }
}