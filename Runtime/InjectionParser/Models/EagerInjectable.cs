using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    public sealed class EagerInjectable<TModel> : InjectableBase<TModel>
    {
        public override object Identifier
        {
            set => Value = Resolve(value);
        }

        public EagerInjectable(TModel value) : base(value)
        {
            InternalValue = value;
        }

        [JsonConstructor]
        private EagerInjectable() { }

        public static implicit operator TModel(EagerInjectable<TModel> injectable) => injectable.Value;
    }
}