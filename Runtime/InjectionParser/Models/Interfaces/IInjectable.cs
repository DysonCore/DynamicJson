using System;

namespace DysonCore.DynamicJson.InjectionParser
{
    internal interface IInjectable<out TModel> : IInjectable
    {
        TModel Value { get; }
        object Identifier { set; }

        Type IInjectable.ModelType => typeof(TModel);

        object IInjectable.GetValue() => Value;
        void IInjectable.SetIdentifier(object identifier) => Identifier = identifier;

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
    
    internal interface IInjectable
    {
        internal Type ModelType { get; }
        
        internal object GetValue();
        internal void SetIdentifier(object identifier);
    }
}