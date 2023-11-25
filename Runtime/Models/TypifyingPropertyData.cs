using System;
using System.Collections.Generic;
using DysonCore.PolymorphicJson.Attributes;

namespace DysonCore.PolymorphicJson.Models
{
    /// <summary>
    /// Represents metadata for properties marked with the <see cref="TypifyingPropertyAttribute"/>. 
    /// Contains information about property type, name, JSON name, and the mapping between values and their respective types, as well as any <see cref="TypifiedPropertyAttribute"/>s associated with this <see cref="TypifyingPropertyAttribute"/>.
    /// </summary>
    internal class TypifyingPropertyData : PropertyData
    {
        internal TypifyingPropertyAttribute TypifyingAttribute { get; }
        internal Dictionary<object, Type> ValuesData { get; }
        internal List<TypifiedPropertyData> TypifiedProperties { get; }

        internal TypifyingPropertyData(Type propertyType, string propertyName, string jsonName, TypifyingPropertyAttribute baseTypifyingAttribute) : base(propertyType, propertyName, jsonName)
        {
            TypifyingAttribute = baseTypifyingAttribute;
            
            ValuesData = new Dictionary<object, Type>();
            TypifiedProperties = new List<TypifiedPropertyData>();
        }
    }
}