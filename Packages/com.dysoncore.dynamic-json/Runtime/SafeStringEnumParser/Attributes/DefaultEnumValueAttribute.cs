using System;

namespace DysonCore.DynamicJson.SafeStringEnumParser
{
    /// <summary>
    /// Marks enum field as a default value for SafeStringEnumConverter.
    /// This value will be used as a fallback in case when matching enum member is missing. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultEnumValueAttribute : Attribute
    {
        
    }
}