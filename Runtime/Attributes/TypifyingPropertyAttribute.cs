using System;
using DysonCore.PolymorphicJson.Enums;

namespace DysonCore.PolymorphicJson.Attributes
{
    /// <summary>
    /// Marks property for polymorphic deserialization as a qualifier.
    /// Should be used on property declaration and value assignment. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class TypifyingPropertyAttribute : Attribute
    {
        public UnknownTypeHandling UnknownTypeHandling { get; }
        public Type InheritanceRoot { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypifyingPropertyAttribute"/> class.
        /// </summary>
        public TypifyingPropertyAttribute()
        {
            UnknownTypeHandling = UnknownTypeHandling.ThrowError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypifyingPropertyAttribute"/> class with explicit inheritance root.
        /// Should be used in derived classes when the base class is interface!
        /// Can be omitted if the base class is abstract or concrete.
        /// </summary>
        public TypifyingPropertyAttribute(Type inheritanceRoot)
        {
            InheritanceRoot = inheritanceRoot;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TypifyingPropertyAttribute"/> class with explicit <see cref="UnknownTypeHandling"/>.
        /// Can be used in the base class.
        /// By default - <see cref="UnknownTypeHandling.ThrowError"/>
        /// </summary>
        public TypifyingPropertyAttribute(UnknownTypeHandling unknownTypeHandling)
        {
            UnknownTypeHandling = unknownTypeHandling;
        }
    }
}