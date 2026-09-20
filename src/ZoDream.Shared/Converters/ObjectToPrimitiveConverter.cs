using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZoDream.Shared.Converters
{
    public class ObjectToPrimitiveConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 根据 JsonTokenType 判断实际类型
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return reader.GetBoolean();
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out int i))
                    {
                        return i;
                    }
                    return reader.GetDouble();
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Null:
                    return null;
                default:
                    // 如果是复杂对象，回退到默认的 JsonElement 处理
                    using (var doc = JsonDocument.ParseValue(ref reader))
                    {
                        return doc.RootElement.Clone();
                    }
            }
        }

        public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value?.GetType() ?? typeof(object), options);
        }
    }
}
