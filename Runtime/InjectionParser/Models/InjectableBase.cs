using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Base class for JSON injectable (<see cref="InjectionConverter"/>) models.
    /// </summary>
    /// <typeparam name="TValue">The type of the model.</typeparam>
    public abstract class InjectableBase<TValue> : IInjectable<TValue>
    {
        protected TValue InternalValue;
        
        /// <summary>
        /// Sets the identifier used to resolve the model value.
        /// </summary>
        protected abstract object Identifier { set; }
        
        /// <summary>
        /// Virtual property to get and set the model value.
        /// Can be overriden to create different resolving behaviours.
        /// </summary>
        public virtual TValue Value 
        { 
            get => InternalValue; 
            protected set => InternalValue = value;
        }

        /// <summary>
        /// Re-route of internal Identifier setter to protected setter.
        /// </summary>
        object IInjectable<TValue>.Identifier
        {
            set => Identifier = value;
        }

        /// <summary>
        /// Initializes a new instance with a given model value.
        /// </summary>
        /// <param name="value">The model value.</param>
        protected InjectableBase(TValue value)
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
        protected static TValue Resolve(object identifier)
        {
            return IInjectable<TValue>.Resolve(identifier);
        }
    }
}