using UnityEditor;

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
            CacheWriter.CreateCache();
        }
        
    }
}