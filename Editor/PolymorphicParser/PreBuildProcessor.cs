using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace DysonCore.DynamicJson.Editor.PolymorphicParser
{
    internal class PreBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            CacheWriter.CreateCache();
        }
    }
}