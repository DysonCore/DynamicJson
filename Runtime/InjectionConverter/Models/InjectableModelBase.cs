using System;
using DysonCore.DynamicJson.InjectionConverter.Registries;

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

        public InjectableModelBase(TModel model)
        {
            Model = model;
        }

        protected abstract void SetIdentifier(object identifier);

        protected static TModel Resolve(object identifier)
        {
            if (!ProviderRegistry.TryGetProvider(typeof(TModel), out IInjectionDataProvider provider))
            {
                //TODO make better exception
                throw new Exception();
            }
            
            if (provider.GetValue(identifier) is not TModel model)
            {
                throw new Exception();
            }

            return model;
        }
    }
}