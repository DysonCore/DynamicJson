using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Base class for JSON injectable (<see cref="InjectionConverter"/>) models.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class InjectableBase<TModel> : IInjectable<TModel>
    {
        protected TModel InternalValue;
        
        /// <summary>
        /// Virtual property to get and set the model value.
        /// </summary>
        public virtual TModel Value 
        { 
            get => InternalValue; 
            protected set => InternalValue = value;
        }

        /// <summary>
        /// Sets the identifier used to resolve the model value.
        /// </summary>
        public abstract object Identifier { set; }

        /// <summary>
        /// Initializes a new instance with a given model value.
        /// </summary>
        /// <param name="value">The model value.</param>
        protected InjectableBase(TModel value)
        {
            InternalValue = value;
        }

        /// <summary>
        /// Initializes a new instance for <see cref="InjectionConverter"/> deserialization.
        /// </summary>
        [JsonConstructor]
        protected InjectableBase() { }

        /// <summary>
        /// Resolves the model value based on the provided identifier.
        /// </summary>
        /// <param name="identifier">The identifier to resolve the value for.</param>
        /// <returns>The resolved model value.</returns>
        protected static TModel Resolve(object identifier)
        {
            return IInjectable<TModel>.Resolve(identifier);
        }
    }
}