using System;

namespace DysonCore.PolymorphicJson.PolymorphicConverter
{
    /// <summary>
    /// Marks property for polymorphic deserialization as a qualifier.
    /// Should be used on property declaration and value assignment. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class TypifyingPropertyAttribute : Attribute
    {
        public Type InheritanceRoot { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypifyingPropertyAttribute"/> class.
        /// </summary>
        public TypifyingPropertyAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypifyingPropertyAttribute"/> class with explicit inheritance root.
        /// Should be used in derived classes when the base class is interface!
        /// Can be omitted if the base class is abstract or concrete.
        /// </summary>
        public TypifyingPropertyAttribute(Type inheritanceRoot)
        {
            InheritanceRoot = inheritanceRoot;
        }
    }
}