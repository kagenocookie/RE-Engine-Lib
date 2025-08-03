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

    public class RszInstanceJsonConverter(Workspace env) : JsonConverter<RszInstance>
    {
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
                var type = field.type switch {
                    RszFieldType.Object or RszFieldType.Struct => typeof(RszInstance),
                    // RszFieldType.UserData => typeof(string), // TODO
                    _ => RszInstance.RszFieldTypeToCSharpType(field.type),
                };
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
                    if (env.IsEmbeddedInstanceInfoUserdata) {
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
