using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DysonCore.PolymorphicJson
{
    internal sealed class IgnoreConvertersContractResolver : DefaultContractResolver
    {
        private readonly Type[] _typesToIgnore;

        internal IgnoreConvertersContractResolver(params Type[] typesToIgnore)
        {
            _typesToIgnore = typesToIgnore ?? Type.EmptyTypes;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            var converter = base.ResolveContractConverter(objectType);

            if (converter != null && _typesToIgnore.Contains(converter.GetType()))
            {
                converter = null;
            }

            return converter;
        }
    }
}