using System;

namespace DysonCore.DynamicJson.InjectionConverter
{
    internal interface IInjectableModel<TModel> : IInjectableModel
    {
        Type IInjectableModel.ModelType => typeof(TModel);
    }

    internal interface IInjectableModel
    {
        internal Type ModelType { get; }
        internal object Identifier { set; }
    }
}