using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Provides lazy initialization for <see cref="IInjectable"/> models.
    /// </summary>
    /// <typeparam name="TValue">The type of the model.</typeparam>
    public sealed class LazyInjectable<TValue> : InjectableBase<TValue>
    {
        private object _identifier;

        /// <summary>
        /// Gets the model value, resolving it lazily if not already set.
        /// </summary>
        public override TValue Value
        {
            get => InternalValue ??= Resolve(_identifier);
            protected set => InternalValue = value;
        }
        
        /// <inheritdoc />
        protected override object Identifier
        {
            set => _identifier = value;
        }

        /// <inheritdoc />
        public LazyInjectable(TValue value)
        {
            InternalValue = value;
        }

        /// <inheritdoc />
        [JsonConstructor]
        private LazyInjectable() { }
        
        /// <summary>
        /// Implicit conversion from LazyInjectable to TModel.
        /// </summary>
        /// <param name="injectable">The LazyInjectable instance.</param>
        public static implicit operator TValue(LazyInjectable<TValue> injectable) => injectable.Value;
    }
}