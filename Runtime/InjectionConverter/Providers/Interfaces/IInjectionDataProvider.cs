using System;

namespace DysonCore.DynamicJson.InjectionConverter
{
    public interface IInjectionDataProvider<in TKey, out TValue> : IInjectionDataProvider
    {
        Type IInjectionDataProvider.KeyType => typeof(TKey);
        Type IInjectionDataProvider.ValueType => typeof(TValue);
        
        TValue GetValue(TKey key);

        object IInjectionDataProvider.GetValue(object key)
        {
            if (key is not TKey explicitKey)
            {
                return null;
            }

            return GetValue(explicitKey);
        }
    }

    public interface IInjectionDataProvider
    {
        internal Type KeyType { get; }
        internal Type ValueType { get; }
        object GetValue(object key);
    }
}