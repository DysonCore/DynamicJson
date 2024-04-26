namespace DysonCore.DynamicJson.PolymorphicParser
{
    public class PolymorphicCacheConstants
    {
        internal const string ResourcesDirectoryName = "Resources";
        internal const string CacheDirectoryName = "DynamicJson";
        internal const string JsonSuffix = ".json";
        internal const string FileName = "PolymorphicCache";

        internal static string FullFileName => string.Concat(FileName, JsonSuffix);
    }
}