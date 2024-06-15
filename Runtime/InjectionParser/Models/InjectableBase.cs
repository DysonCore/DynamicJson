using System;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.InjectionParser
{
    public abstract class InjectableBase<TModel> : IInjectable<TModel>
    {
        protected TModel InternalValue;
        
        public virtual TModel Value { get => InternalValue; protected set => InternalValue = value; }

        object IInjectable.Identifier
        {
            set => SetIdentifier(value);
        }

        protected InjectableBase(TModel value)
        {
            InternalValue = value;
        }

        [JsonConstructor]
        protected InjectableBase() { }
        
        protected abstract void SetIdentifier(object identifier);

        public static implicit operator TModel(InjectableBase<TModel> injectable) => injectable.Value;
        
        protected static TModel Resolve(object identifier)
        {
            IInjectionDataProvider provider = ProviderRegistry.GetProvider(typeof(TModel));
            
            if (provider.GetValue(identifier) is not TModel model)
            {
                throw new Exception();
            }

            return model;
        }
    }
}