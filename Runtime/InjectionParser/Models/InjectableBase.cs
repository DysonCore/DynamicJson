using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    public abstract class InjectableBase<TModel> : IInjectable<TModel>
    {
        protected TModel InternalValue;
        
        public virtual TModel Value 
        { 
            get => InternalValue; 
            protected set => InternalValue = value;
        }

        public abstract object Identifier { set; }

        protected InjectableBase(TModel value)
        {
            InternalValue = value;
        }

        [JsonConstructor]
        protected InjectableBase() { }

        protected static TModel Resolve(object identifier)
        {
            return IInjectable<TModel>.Resolve(identifier);
        }
    }
}
