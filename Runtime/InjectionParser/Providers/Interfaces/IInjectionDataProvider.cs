using System;

namespace DysonCore.DynamicJson.InjectionParser
{
    internal interface IInjectionDataProvider<TIdentifier, TValue> : IInjectionDataProvider
    {
        TValue GetValue(TIdentifier identifier);
        TIdentifier GetIdentifier(TValue value);
        
        Type IInjectionDataProvider.IdentifierType => typeof(TIdentifier);
        Type IInjectionDataProvider.ValueType => typeof(TValue);

        object IInjectionDataProvider.GetValue(object identifier)
        {
            return identifier is TIdentifier actualIdentifier ? GetValue(actualIdentifier) : null;
        }
        
        object IInjectionDataProvider.GetIdentifier(object value)
        {
            return value is TValue actualValue ? GetIdentifier(actualValue) : null;
        }
    }

    internal interface IInjectionDataProvider
    {
        internal Type IdentifierType { get; }
        internal Type ValueType { get; }
        
        internal object GetValue(object identifier);
        internal object GetIdentifier(object value);
    }
}