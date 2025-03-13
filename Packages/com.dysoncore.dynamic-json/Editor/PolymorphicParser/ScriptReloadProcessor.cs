using System;
using UnityEditor;

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

        }
    }
}