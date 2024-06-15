using System;

namespace DysonCore.DynamicJson.InjectionParser
{
    internal interface IInjectable<out TModel> : IInjectable
    {
        TModel Value { get; }
        
        Type IInjectable.ModelType => typeof(TModel);

        object IInjectable.GetValue() => Value;
    }
    
    internal interface IInjectable
    {
        internal Type ModelType { get; }
        internal object Identifier { set; }
        internal object GetValue();
    }
}