using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Provides lazy initialization for <see cref="IInjectable"/> models.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public sealed class LazyInjectable<TModel> : InjectableBase<TModel>
    {
        private object _identifier;

        /// <summary>
        /// Gets the model value, resolving it lazily if not already set.
        /// </summary>
        public override TModel Value
        {
            get => InternalValue ??= Resolve(_identifier);
            protected set => InternalValue = value;
        }
        
        /// <inheritdoc />
        public override object Identifier
        {
            set => _identifier = value;
        }

        /// <inheritdoc />
        public LazyInjectable(TModel value)
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
        public static implicit operator TModel(LazyInjectable<TModel> injectable) => injectable.Value;
    }
}