using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    public class LazyInjectable<TModel> : InjectableBase<TModel>
    {
        private object _identifier;

        public sealed override TModel Value
        {
            get => InternalValue ??= Resolve(_identifier);
            protected set => InternalValue = value;
        }

        public LazyInjectable(TModel value)
        {
            InternalValue = value;
        }

        [JsonConstructor]
        private LazyInjectable() { }

        protected sealed override void SetIdentifier(object identifier)
        {
            _identifier = identifier;
        }
    }
}