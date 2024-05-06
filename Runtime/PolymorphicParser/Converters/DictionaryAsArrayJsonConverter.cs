using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

internal sealed class DictionaryAsArrayJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is not IDictionary dictionary)
        {
            throw new Exception($"[{nameof(DictionaryAsArrayJsonConverter)}.{nameof(WriteJson)}] cant write {nameof(value)} which is not {nameof(IDictionary)}.");
        }

        writer.WriteStartArray();

        IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
        using (enumerator as IDisposable)
        {
            while (enumerator.MoveNext())
            {
                writer.WriteStartArray();
                serializer.Serialize(writer, enumerator.Key);
                serializer.Serialize(writer, enumerator.Value);
                writer.WriteEndArray();
            }
        }
        
        writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        Type keyType = null;
        Type valueType = null;
        Type actualType = objectType;
        IDictionary result;

        if (objectType.IsGenericType)
        {
            keyType = objectType.GetGenericArguments()[0];
            valueType = objectType.GetGenericArguments()[1];
            actualType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        }
        
        result = (IDictionary)Activator.CreateInstance(actualType);

        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }
            

        int depth = reader.Depth;
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    break;
                case JsonToken.EndArray:
                {
                    if (reader.Depth == depth)
                    {
                        return result;
                    }
                        
                    break;
                }
                default:
                {
                    object key = serializer.Deserialize(reader, keyType);
                    reader.Read();
                    object value = serializer.Deserialize(reader, valueType);
                    
                    if (key != null)
                    {
                        result.Add(key, value);
                    }
                    break;
                }
            }
        }

        return result;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(IDictionary).IsAssignableFrom(objectType);
    }
}