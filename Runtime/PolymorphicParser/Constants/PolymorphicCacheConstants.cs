namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal static class PolymorphicCacheConstants
    {
        internal const string AssetsDirectoryName = "Assets";
        internal const string ResourcesDirectoryName = "Resources";
        internal const string CacheDirectoryName = "DynamicJson";
        internal const string FileName = "PolymorphicCache";
        
        private const string JsonSuffix = ".json";

        internal static string FullFileName => string.Concat(FileName, JsonSuffix);
    }
}