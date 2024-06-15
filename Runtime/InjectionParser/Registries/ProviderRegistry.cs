using System;
using System.Collections.Concurrent;

namespace DysonCore.DynamicJson.InjectionParser
{
    internal static class ProviderRegistry
    {
        //Concurrent Dictionary for thread safety 
        private static readonly ConcurrentDictionary<Type, IInjectionDataProvider> ModelToProviderData = new ();

        internal static void AddProvider(IInjectionDataProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            if (ModelToProviderData.TryAdd(provider.ValueType, provider) is false)
            {
                throw new Exception($"[{nameof(ProviderRegistry)}.{nameof(AddProvider)}] Instance of {provider.GetType().Name} already exists. Use only one instance! ");
            }
        }
        
        internal static void RemoveProvider(IInjectionDataProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            ModelToProviderData.TryRemove(provider.ValueType, out _);
        }

        internal static IInjectionDataProvider GetProvider(Type modelType)
        {
            ModelToProviderData.TryGetValue(modelType, out IInjectionDataProvider provider);

            if (provider == null)
            {
                throw new Exception($"[{nameof(ProviderRegistry)}.{nameof(GetProvider)}] Instance with {nameof(modelType)} {modelType.Name} can not be found. Make sure {nameof(IInjectionDataProvider)} with given {nameof(modelType)} is initialized before deserialization!");
            }

            return provider;
        }
    }
}