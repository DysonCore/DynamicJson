using System;
using DysonCore.PolymorphicJson.Enums;

namespace DysonCore.PolymorphicJson
{
    /// <summary>
    /// Marks property for polymorphic deserialization as a qualifier.
    /// Should be used on property declaration and value assignment. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class TypifyingPropertyAttribute : Attribute
    {
        public UnknownTypeHandling UnknownTypeHandling { get; }

        public TypifyingPropertyAttribute(UnknownTypeHandling unknownTypeHandling = UnknownTypeHandling.ThrowError)
        {
            UnknownTypeHandling = unknownTypeHandling;
        }
    }
}