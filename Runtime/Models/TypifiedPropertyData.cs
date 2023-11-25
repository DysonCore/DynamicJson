using System;

namespace DysonCore.PolymorphicJson.Models
{
    internal class TypifiedPropertyData : PropertyData
    {
        internal TypifiedPropertyData(Type propertyType, string propertyName, string jsonName) : base(propertyType, propertyName, jsonName)
        {
        }
    }
}