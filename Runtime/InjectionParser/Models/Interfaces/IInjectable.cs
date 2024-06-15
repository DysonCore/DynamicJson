using System;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Interface for JSON injectable (<see cref="InjectionConverter"/>) models with a generic model <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TModel">The <see cref="Type"/> of the model.</typeparam>
    internal interface IInjectable<out TModel> : IInjectable
    {
        /// <summary>
        /// Gets the model value.
        /// </summary>
        TModel Value { get; }

        /// <summary>
        /// Sets the identifier used to resolve the model value.
        /// </summary>
        object Identifier { set; }

        Type IInjectable.ModelType => typeof(TModel);

        object IInjectable.GetValue() => Value;
        void IInjectable.SetIdentifier(object identifier) => Identifier = identifier;

        /// <summary>
        /// Resolves the model value based on the provided identifier.
        /// </summary>
        /// <param name="identifier">The identifier to resolve the value for.</param>
        /// <returns>The resolved model value.</returns>
        protected static TModel Resolve(object identifier)
        {
            IInjectionDataProvider provider = ProviderRegistry.GetProvider(typeof(TModel));
            if (provider.GetValue(identifier) is not TModel model)
            {
                throw new Exception();
            }

            return model;
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