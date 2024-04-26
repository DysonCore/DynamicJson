using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

internal sealed class DictionaryAsArrayJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var dictionary = (IDictionary)value;

        writer.WriteStartArray();

        var en = dictionary.GetEnumerator();
        while (en.MoveNext())
        {
            writer.WriteStartArray();
            serializer.Serialize(writer, en.Key);
            serializer.Serialize(writer, en.Value);
            writer.WriteEndArray();
        }
        
        writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (!CanConvert(objectType))
            throw new Exception(string.Format("This converter is not for {0}.", objectType));

        Type keyType = null;
        Type valueType = null;
        IDictionary result;

        if (objectType.IsGenericType)
        {
            keyType = objectType.GetGenericArguments()[0];
            valueType = objectType.GetGenericArguments()[1];
            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            result = (IDictionary)Activator.CreateInstance(dictionaryType);
        }
        else
        {
            result = (IDictionary)Activator.CreateInstance(objectType);
        }

        if (reader.TokenType == JsonToken.Null)
            return null;

        int depth = reader.Depth;
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
            }
            else if (reader.TokenType == JsonToken.EndArray)
            {
                if (reader.Depth == depth)
                    return result;
            }
            else
            {
                object key = serializer.Deserialize(reader, keyType);
                reader.Read();
                object value = serializer.Deserialize(reader, valueType);
                result.Add(key, value);
            }
        }

        return result;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(IDictionary).IsAssignableFrom(objectType);
    }

}