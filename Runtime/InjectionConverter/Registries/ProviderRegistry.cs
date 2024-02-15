using System;
using System.Collections.Concurrent;

namespace DysonCore.DynamicJson.InjectionConverter
{
    internal static class ProviderRegistry
    {
        private static readonly ConcurrentDictionary<Type, IInjectionDataProvider> ModelToProviderData = new ();

        internal static void AddProvider(IInjectionDataProvider provider)
        {
            if (provider is null)
            {
                return;
            }

            ModelToProviderData.TryAdd(provider.ValueType, provider);
        }
        
        internal static void RemoveProvider(IInjectionDataProvider provider)
        {
            if (provider is null)
            {
                return;
            }

            ModelToProviderData.TryRemove(provider.ValueType, out _);
        }

        internal static bool TryGetProvider(Type key, out IInjectionDataProvider provider)
        {
            return ModelToProviderData.TryGetValue(key, out provider);
        }
    }
}