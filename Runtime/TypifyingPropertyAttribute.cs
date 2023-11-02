using System;

namespace DysonCore.PolymorphicJson
{
    /// <summary>
    /// Marks property for polymorphic deserialization as a qualifier.
    /// Should be used on property declaration and value assignment. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class TypifyingPropertyAttribute : Attribute
    {
        
    }
}