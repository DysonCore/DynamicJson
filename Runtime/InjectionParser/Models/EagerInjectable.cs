using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// Provides eager initialization for <see cref="IInjectable"/> models.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public sealed class EagerInjectable<TModel> : InjectableBase<TModel>
    {
        /// <summary>
        /// Sets the identifier and immediately resolves the model value.
        /// </summary>
        public override object Identifier
        {
            set => Value = Resolve(value);
        }

        /// <inheritdoc />
        public EagerInjectable(TModel value) : base(value)
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
        public static implicit operator TModel(EagerInjectable<TModel> injectable) => injectable.Value;
    }
}