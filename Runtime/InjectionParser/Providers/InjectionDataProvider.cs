namespace DysonCore.DynamicJson.InjectionConverter
{
    public abstract class InjectionDataProvider<TModel, TIdentifier> : IInjectionDataProvider<TModel, TIdentifier>
    {
        protected InjectionDataProvider()
        {
            ProviderRegistry.AddProvider(this);
        }

        ~InjectionDataProvider()
        {
            ProviderRegistry.RemoveProvider(this);
        }
        
        public abstract TModel GetValue(TIdentifier key);
    }
}