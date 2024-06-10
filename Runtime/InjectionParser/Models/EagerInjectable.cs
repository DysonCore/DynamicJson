namespace DysonCore.DynamicJson.InjectionConverter
{
    public class EagerInjectable<TModel> : InjectableBase<TModel>
    {
        protected sealed override void SetIdentifier(object identifier)
        {
            Value = Resolve(identifier);
        }
    }
}