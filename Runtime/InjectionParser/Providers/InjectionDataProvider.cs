namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Base class for creating <see cref="IInjectionDataProvider"/>s.
    /// </summary>
    /// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
    /// <typeparam name="TValue">The type of the value to provide.</typeparam>
    public abstract class InjectionDataProvider<TIdentifier, TValue> : IInjectionDataProvider<TIdentifier, TValue>
    {
        /// <summary>
        /// Initializes a new instance of provider and registers it in the <see cref="ProviderRegistry"/>.
        /// </summary>
        protected InjectionDataProvider()
        {
            ProviderRegistry.AddProvider(this);
        }

        /// <summary>
        /// Destructor that removes the provider from the <see cref="ProviderRegistry"/>.
        /// </summary>
        ~InjectionDataProvider()
        {
            ProviderRegistry.RemoveProvider(this);
        }
        
        
        /// <inheritdoc />
        public abstract TValue GetValue(TIdentifier key);

        /// <inheritdoc />
        public abstract TIdentifier GetIdentifier(TValue value);
    }
}