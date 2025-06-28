using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ReeLib.Common
{
    public class HexUIntJsonConverter : JsonConverter<uint>
    {
        public override uint Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? text = reader.GetString();
                if (text == null) return default;
                return Convert.ToUInt32(text, 16);
            }

            return reader.GetUInt32();
        }

        public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"0x{value:x}");
        }
    }

    public class HexIntJsonConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? text = reader.GetString();
                if (text == null) return default;
                return Convert.ToInt32(text, 16);
            }

            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"0x{value:x}");
        }
    }

    public class HexIntPtrJsonConverter : JsonConverter<nint>
    {
        public override nint Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? text = reader.GetString();
                if (text == null) return default;
                return (nint)Convert.ToInt64(text, 16);
            }

            return (nint)reader.GetInt64();
        }

        public override void Write(Utf8JsonWriter writer, nint value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"0x{value:x}");
        }
    }

    public class ConstObjectConverter : JsonConverter<object?>
    {
        public override object? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (value is string stringValue)
            {
                writer.WriteStringValue(stringValue);
            }
            else if (value is IConvertible convertible)
            {
                writer.WriteNumberValue(convertible.ToDecimal(null));
            }
        }
    }


    public class EnumJsonConverter<T> : JsonConverter<T> where T : unmanaged
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override T Read(ref Utf8JsonReader reader, Type enumType, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? text = reader.GetString();
                if (text != null)
                {
#if NET5_0_OR_GREATER
                    return Enum.Parse<T>(text);
#else
                    return (T)Enum.Parse(typeof(T), text);
#endif
                }
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class RszInstanceJsonConverter(Workspace env) : JsonConverter<RszInstance>
    {
        private JsonSerializerOptions? recursionPair;

        public override RszInstance? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
            if (dict?.Remove("$type", out var classnameField) != true || classnameField?.GetValue<string>() is not string classname) {
                throw new Exception("Missing $type property in JSON for RSZ instance");
            }
            var cls = env.RszParser.GetRSZClass(classname!) ?? throw new Exception("Can't deserialize unknown RSZ class ");
            var inst = RszInstance.CreateInstance(env.RszParser, cls, -1);
            foreach (var (key, val) in dict) {
                var fieldIdx = cls.IndexOfField(key);
                if (fieldIdx == -1) {
                    // error?
                    continue;
                }
                var field = cls.fields[fieldIdx];
                var type = RszInstance.RszFieldTypeToCSharpType(field.type);
                if (field.array) {
                    var list = inst.Values[fieldIdx] as List<object> ?? new List<object>();
                    list.Clear();
                    if (field.type == RszFieldType.Object || field.type == RszFieldType.Struct) {
                        if (val!.AsObject().TryGetPropertyValue("items", out var items)) {
                            foreach (var sub in items!.AsArray()) {
                                list.Add(sub.Deserialize<RszInstance>(options)!);
                            }
                        } else {
                            foreach (var sub in val!.AsArray()) {
                                list.Add(sub.Deserialize<RszInstance>(options)!);
                            }
                        }
                    } else {
                        foreach (var sub in val!.AsArray()) {
                            list.Add(sub.Deserialize(type, options)!);
                        }
                    }
                    inst.SetFieldValue(key, list);
                } else {
                    inst.SetFieldValue(key, val.Deserialize(type, options)!);
                }
            }
            return inst;
        }

        public override void Write(Utf8JsonWriter writer, RszInstance value, JsonSerializerOptions options)
        {
            var dict = new Dictionary<string, object>(value.Fields.Length + 1);
            // TODO handle userdata
            // TODO handle dictionary arrays (KeyValuePair) as dictionaries
            dict["$type"] = value.RszClass.name;
            for (var i = 0; i < value.Fields.Length; i++) {
                var field = value.Fields[i];
                var fieldValue = value.Values[i];
                if (field.type == RszFieldType.Object && field.array) {
                    // store as structure: {"$array": "base class type", "items": []}
                    // we need it for diffs, because the instance types might not be equal to the base type
                    dict[field.name] = new Dictionary<string, object>() {
                        { "$array", field.original_type },
                        { "items", fieldValue },
                    };
                } else if (field.type == RszFieldType.UserData) {
                    // var rszField = (RszInstance)fieldValue;
                    if (env.IsInlineUserdata) {
                        if (field.array) {
                            // TODO
                        } else {
                            // TODO
                        }
                    } else if (env.IsEmbeddedUserdata) {
                        // TODO
                    } else {
                        // TODO
                    }
                } else {
                    dict[field.name] = fieldValue;
                }
            }
            JsonSerializer.Serialize(writer, dict, options);
        }
    }

}
