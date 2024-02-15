namespace DysonCore.DynamicJson.InjectionConverter
{
    public class EagerInjectableModel<TModel> : InjectableModelBase<TModel>
    {
        protected sealed override void SetIdentifier(object identifier)
        {
            Value = Resolve(identifier);
        }

        public EagerInjectableModel(TModel model) : base(model)
        {
        }
    }
}