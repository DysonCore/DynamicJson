using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    public sealed class LazyInjectable<TModel> : InjectableBase<TModel>
    {
        private object _identifier;

        public override TModel Value
        {
            get => InternalValue ??= Resolve(_identifier);
            protected set => InternalValue = value;
        }
        
        public override object Identifier
        {
            set => _identifier = value;
        }

        public LazyInjectable(TModel value)
        {
            InternalValue = value;
        }

        [JsonConstructor]
        private LazyInjectable() { }
        
        public static implicit operator TModel(LazyInjectable<TModel> injectable) => injectable.Value;
    }
}