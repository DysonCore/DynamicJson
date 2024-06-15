using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DysonCore.DynamicJson.InjectionParser
{
    /// <summary>
    /// JSON converter for objects implementing the <see cref="IInjectable"/> interface.
    /// </summary>
    public class InjectionConverter : JsonConverter
    {
        /// <summary>
        /// Type to convert by <see cref="InjectionConverter"/>.
        /// </summary>
        private Type ConvertableType => typeof(IInjectable);
        
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not IInjectable injectable) // impossible case.
            {
                throw new JsonWriterException($"[{nameof(InjectionConverter)}.{nameof(WriteJson)}] {nameof(InjectionConverter)} has received {nameof(Type)} for serialization which it can't process. \n{nameof(Type)} - {value?.GetType().Name}");
            }

            IInjectionDataProvider provider = ProviderRegistry.GetProvider(injectable.ModelType);
            object modelValue = injectable.GetValue();
            object identifier = provider.GetIdentifier(modelValue);
            
            serializer.Serialize(writer, identifier);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.IsAbstract)
            {
                throw new JsonReaderException($"[{nameof(InjectionConverter)}.{nameof(ReadJson)}] {nameof(IInjectable)} property is abstract! Use concrete implementations!.\n{nameof(objectType)} - {objectType.Name}.");
            }

            object instance = Activator.CreateInstance(objectType, true);

            if (instance is not IInjectable injectable) // impossible case.
            {
                throw new JsonReaderException($"[{nameof(InjectionConverter)}.{nameof(ReadJson)}] {nameof(InjectionConverter)} has received {nameof(Type)} for deserialization which it can't process. \n{nameof(Type)} - {objectType.Name}");
            }
            
            JToken token = JToken.Load(reader);
            IInjectionDataProvider provider = ProviderRegistry.GetProvider(injectable.ModelType);
            
            object identifier = token.ToObject(provider.IdentifierType, serializer);
            injectable.SetIdentifier(identifier);

            return injectable;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return ConvertableType.IsAssignableFrom(objectType);
        }
    }
}