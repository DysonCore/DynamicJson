using System;

namespace DysonCore.PolymorphicJson.Attributes
{
    /// <summary>
    /// Marks property for polymorphic deserialization as a property qualified by <see cref="TypifyingPropertyAttribute"/>.
    /// Should be used on polymorphic properties in the same root class which declares property with <see cref="TypifyingPropertyAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class TypifiedPropertyAttribute : Attribute
    {
        
    }
}