namespace DysonCore.DynamicJson.InjectionConverter
{
    public abstract class InjectionDataProvider<TKey, TValue> : IInjectionDataProvider<TKey, TValue>
    {
        protected InjectionDataProvider()
        {
            ProviderRegistry.AddProvider(this);
        }

        ~InjectionDataProvider()
        {
            ProviderRegistry.RemoveProvider(this);
        }
        
        public abstract TValue GetValue(TKey key);
    }
}