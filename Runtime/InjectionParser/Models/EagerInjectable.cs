using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Provides eager initialization for <see cref="IInjectable"/> models.
    /// </summary>
    /// <typeparam name="TValue">The type of the model.</typeparam>
    public sealed class EagerInjectable<TValue> : InjectableBase<TValue>
    {
        /// <summary>
        /// Sets the identifier and immediately resolves the model value.
        /// </summary>
        protected override object Identifier
        {
            set => Value = Resolve(value);
        }

        /// <inheritdoc />
        public EagerInjectable(TValue value) : base(value)
        {
            InternalValue = value;
        }

        /// <inheritdoc />
        [JsonConstructor]
        private EagerInjectable() { }

        /// <summary>
        /// Implicit conversion from EagerInjectable to TModel.
        /// </summary>
        /// <param name="injectable">The EagerInjectable instance.</param>
        public static implicit operator TValue(EagerInjectable<TValue> injectable) => injectable.Value;
    }
}