using System;

namespace DysonCore.PolymorphicJson.SafeStringEnumConverter
{
    /// <summary>
    /// Marks enum field as a default value for SafeStringEnumConverter.
    /// This value will be used as a fallback in case when there are no matching fields. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultEnumValueAttribute : Attribute
    {
        
    }
}