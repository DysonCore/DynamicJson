using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.InjectionConverter
{
    public class InjectionConverter : JsonConverter
    {
        private Type ConvertableType => typeof(IInjectableModel);
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == null || objectType.IsAbstract)
            {
                throw new Exception();
            }

            object instance = Activator.CreateInstance(objectType);

            if (instance is not IInjectableModel injectableModel)
            {
                throw new Exception();
            }
            
            JToken token = JToken.Load(reader);
            ProviderRegistry.TryGetProvider(injectableModel.ModelType, out IInjectionDataProvider provider);
            
            injectableModel.Identifier = token.ToObject(provider.KeyType, serializer);

            return injectableModel;
        }

        public override bool CanConvert(Type objectType)
        {
            return ConvertableType.IsAssignableFrom(objectType);
        }
    }
}