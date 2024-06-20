using System;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Interface for data providers with generic identifier and value types.
    /// </summary>
    /// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
    /// <typeparam name="TValue">The type of th value.</typeparam>
    internal interface IInjectionDataProvider<TIdentifier, TValue> : IInjectionDataProvider
    {
        /// <summary>
        /// Retrieves a value based on the provided identifier.
        /// </summary>
        /// <param name="identifier">The identifier to get the value for.</param>
        /// <returns>The value associated with the identifier.</returns>
        TValue GetValue(TIdentifier identifier);

        /// <summary>
        /// Retrieves an identifier based on the provided value.
        /// </summary>
        /// <param name="value">The value to get the identifier for.</param>
        /// <returns>The identifier associated with the value.</returns>
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

    /// <summary>
    /// Base interface for data providers.
    /// </summary>
    internal interface IInjectionDataProvider
    {
        /// <summary>
        /// Gets the type of the identifier.
        /// </summary>
        internal Type IdentifierType { get; }
        
        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        internal Type ValueType { get; }
        
        /// <summary>
        /// Retrieves a value based on the provided identifier.
        /// </summary>
        /// <param name="identifier">The identifier to get the value for.</param>
        /// <returns>The value associated with the identifier.</returns>
        internal object GetValue(object identifier);

        /// <summary>
        /// Retrieves an identifier based on the provided value.
        /// </summary>
        /// <param name="value">The value to get the identifier for.</param>
        /// <returns>The identifier associated with the value.</returns>
        internal object GetIdentifier(object value);
    }
}