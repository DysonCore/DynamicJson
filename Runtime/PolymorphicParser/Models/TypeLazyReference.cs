using System;

namespace DysonCore.DynamicJson.PolymorphicParser
{
    internal sealed class TypeLazyReference
    {
        private string _typeName;
        private Type _type;
        
        internal Type Type => _type == null && string.IsNullOrWhiteSpace(_typeName) ? null : _type ??= Type.GetType(_typeName);
        
        internal string TypeName
        {
            get => _type.AssemblyQualifiedName;
            private set => _typeName = value;
        }

        private TypeLazyReference(Type type)
        {
            _type = type;
        }

        internal TypeLazyReference(string typeName)
        {
            TypeName = typeName;
        }
        
        public static explicit operator Type(TypeLazyReference lazyReference)
        {
            return lazyReference.Type;
        }

        public static explicit operator TypeLazyReference(Type type)
        {
            return new TypeLazyReference(type);
        }
        
        private bool Equals(TypeLazyReference other)
        {
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (obj is not TypeLazyReference wrapper)
            {
                return false;
            }

            return ReferenceEquals(this, wrapper) || Equals(wrapper);
        }

        public override int GetHashCode()
        {
            return Type != null ? Type.GetHashCode() : 0;
        }
    }
}