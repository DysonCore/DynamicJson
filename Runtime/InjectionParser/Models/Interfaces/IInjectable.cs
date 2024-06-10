using System;

namespace DysonCore.DynamicJson.InjectionConverter
{
    internal interface IInjectable<TModel> : IInjectable
    {
        Type IInjectable.ModelType => typeof(TModel);
    }

    internal interface IInjectable
    {
        internal Type ModelType { get; }
        internal object Identifier { set; }
    }
}