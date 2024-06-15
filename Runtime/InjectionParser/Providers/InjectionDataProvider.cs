namespace DysonCore.DynamicJson.InjectionParser
{
    public abstract class InjectionDataProvider<TIdentifier, TModel> : IInjectionDataProvider<TIdentifier, TModel>
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
        public abstract TIdentifier GetIdentifier(TModel value);
    }
}