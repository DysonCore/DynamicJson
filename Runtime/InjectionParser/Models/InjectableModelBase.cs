using System;

namespace DysonCore.DynamicJson.InjectionConverter
{
    public abstract class InjectableModelBase<TModel> : IInjectableModel<TModel>
    {
        protected TModel Model;
        public virtual TModel Value 
        { 
            get => Model;
            protected set => Model = value;
        }

        object IInjectableModel.Identifier
        {
            set => SetIdentifier(value);
        }

        public static implicit operator TModel(InjectableModelBase<TModel> injectableModel) => injectableModel.Value;

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