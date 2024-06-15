using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Manages a registry of <see cref="IInjectionDataProvider"/>s.
    /// </summary>
    internal static class ProviderRegistry
    {
        // Concurrent Dictionary for thread safety.
        private static readonly ConcurrentDictionary<Type, IInjectionDataProvider> ModelToProviderData = new ();

        /// <summary>
        /// Adds a new <see cref="IInjectionDataProvider"/> to the <see cref="ProviderRegistry"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IInjectionDataProvider"/> to add.</param>
        /// <exception cref="InvalidOperationException">Thrown when a <see cref="IInjectionDataProvider"/> with the same value <see cref="Type"/> is already added to the <see cref="ProviderRegistry"/>.</exception>
        internal static void AddProvider(IInjectionDataProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            if (ModelToProviderData.TryAdd(provider.ValueType, provider) is false)
            {
                throw new InvalidOperationException($"[{nameof(ProviderRegistry)}.{nameof(AddProvider)}] Instance of {provider.GetType().Name} already exists. Use only one instance! ");
            }
        }

        /// <summary>
        /// Removes a <see cref="IInjectionDataProvider"/> from the <see cref="ProviderRegistry"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IInjectionDataProvider"/> to remove.</param>
        internal static void RemoveProvider(IInjectionDataProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            ModelToProviderData.TryRemove(provider.ValueType, out _);
        }

        /// <summary>
        /// Retrieves a provider for a specified type.
        /// </summary>
        /// <param name="modelType">The type of the model.</param>
        /// <returns>The provider for the specified type.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the provider for the given type is not found.</exception>
        internal static IInjectionDataProvider GetProvider(Type modelType)
        {
            ModelToProviderData.TryGetValue(modelType, out IInjectionDataProvider provider);

            if (provider == null)
            {
                throw new KeyNotFoundException($"[{nameof(ProviderRegistry)}.{nameof(GetProvider)}] Instance with {nameof(modelType)} {modelType.Name} can not be found. Make sure {nameof(IInjectionDataProvider)} with given {nameof(modelType)} is initialized before deserialization!");
            }

            return provider;
        }
    }
}