namespace DysonCore.DynamicJson.PolymorphicParser
{
    /// <summary>
    /// Represents metadata for typified properties. 
    /// Contains information about property type, name, JSON name.
    /// </summary>
    internal class TypifiedPropertyData : PropertyData
    {
        internal TypifiedPropertyData(TypeLazyReference propertyType, string propertyName, string jsonName) : base(propertyType, propertyName, jsonName)
        {
        }
    }
}