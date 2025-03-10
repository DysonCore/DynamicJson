using UnityEditor;
using UnityEngine;
using DysonCore.DynamicJson.CodeGeneration;

namespace DysonCore.DynamicJson.Editor.PolymorphicParser
{
    [InitializeOnLoad]
    internal static class ScriptReloadProcessor
    {

        static ScriptReloadProcessor()
        {
            Test();
            
            OnScriptsReloaded();
        }

        private static void OnScriptsReloaded()
        {
            CacheWriter.CreateCache();
        }

        private static void Test()
        {
            Debug.Log($"{PolymorphicCacheTest.GetTestText()}");
            //var cahce = DefaultEnumValueCache.Cache;
        }
    }
}