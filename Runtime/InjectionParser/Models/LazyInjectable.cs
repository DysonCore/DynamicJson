namespace DysonCore.DynamicJson.InjectionConverter
{
    public class LazyInjectable<TModel> : InjectableBase<TModel>
    {
        private object _identifier;

        public sealed override TModel Value
        {
            get => Model ??= Resolve(_identifier);
            protected set => Model = value;
        }

        protected sealed override void SetIdentifier(object identifier)
        {
            _identifier = identifier;
        }
    }
}