using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    public class EagerInjectable<TModel> : InjectableBase<TModel>
    {
        protected sealed override void SetIdentifier(object identifier)
        {
            Value = Resolve(identifier);
        }

        public EagerInjectable(TModel value) : base(value)
        {
            InternalValue = value;
        }

        [JsonConstructor]
        private EagerInjectable() { }
    }
}