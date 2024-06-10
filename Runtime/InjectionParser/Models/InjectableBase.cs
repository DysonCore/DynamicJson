using System;

namespace DysonCore.DynamicJson.InjectionConverter
{
    public abstract class InjectableBase<TModel> : IInjectable<TModel>
    {
        protected TModel Model;
        public virtual TModel Value 
        { 
            get => Model;
            protected set => Model = value;
        }

        object IInjectable.Identifier
        {
            set => SetIdentifier(value);
        }

        public static implicit operator TModel(InjectableBase<TModel> injectable) => injectable.Value;

        protected abstract void SetIdentifier(object identifier);

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