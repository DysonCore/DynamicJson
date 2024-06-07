using System;

namespace DysonCore.DynamicJson.InjectionConverter
{
    public interface IInjectionDataProvider<in TModel, out TIdentifier> : IInjectionDataProvider
    {
        Type IInjectionDataProvider.ModelType => typeof(TModel);
        Type IInjectionDataProvider.IdentifierType => typeof(TIdentifier);
        
        TIdentifier GetValue(TModel key);

        object IInjectionDataProvider.GetValue(object identifier)
        {
            return identifier is TIdentifier actualIdentifier ? GetValue(actualIdentifier) : null;
        }
    }

    public interface IInjectionDataProvider
    {
        internal Type ModelType { get; }
        internal Type IdentifierType { get; }
        internal object GetValue(object key);
    }
}