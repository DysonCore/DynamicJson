using System;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Interface for JSON injectable (<see cref="InjectionConverter"/>) models with a generic model <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TValue">The <see cref="Type"/> of the model.</typeparam>
    internal interface IInjectable<out TValue> : IInjectable
    {
        /// <summary>
        /// Gets the model value.
        /// </summary>
        TValue Value { get; }

        /// <summary>
        /// Sets the identifier used to resolve the model value.
        /// </summary>
        protected object Identifier { set; }

        Type IInjectable.ModelType => typeof(TValue);
        object IInjectable.GetValue() => Value;
        void IInjectable.SetIdentifier(object identifier) => Identifier = identifier;

        /// <summary>
        /// Resolves the model value based on the provided identifier.
        /// </summary>
        /// <param name="identifier">The identifier to resolve the value for.</param>
        /// <returns>The resolved model value.</returns>
        protected static TValue Resolve(object identifier)
        {
            IInjectionDataProvider provider = ProviderRegistry.GetProvider(typeof(TValue));
            if (provider.GetValue(identifier) is not TValue value)
            {
                throw new Exception();
            }

            return value;
        }
    }
    
    /// <summary>
    /// Base interface for JSON injectable (<see cref="InjectionConverter"/>) models.
    /// </summary>
    internal interface IInjectable
    {
        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        internal Type ModelType { get; }
        
        /// <summary>
        /// Gets the model value.
        /// </summary>
        /// <returns>The model value.</returns>
        internal object GetValue();

        /// <summary>
        /// Sets the identifier used to resolve the model value.
        /// </summary>
        /// <param name="identifier">The identifier to set.</param>
        internal void SetIdentifier(object identifier);
    }
}