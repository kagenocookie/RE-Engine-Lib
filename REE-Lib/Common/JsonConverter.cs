using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ReeLib.Pfb;
using ReeLib.Scn;

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
                var type = RszInstance.RszFieldTypeToRuntimeCSharpType(field.type);
                if (field.array) {
                    var list = inst.Values[fieldIdx] as List<object> ?? new List<object>();
                    list.Clear();
                    if (field.type == RszFieldType.Object || field.type == RszFieldType.Struct) {
                        if (val?.GetValueKind() == JsonValueKind.Object && val.AsObject().TryGetPropertyValue("items", out var items)) {
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
                } else if (field.type == RszFieldType.UserData) {
                    if (field.array) {
                        var list = new List<object>(val?.AsArray().Select(DeserializeUserdata) ?? []);
                        inst.SetFieldValue(key, list);
                    } else {
                        inst.SetFieldValue(key, DeserializeUserdata(val));
                    }
                } else {
                    var deserialized = val?.GetValueKind() switch {
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Number => val.Deserialize(type, options),
                        JsonValueKind.Null or JsonValueKind.Undefined => null,
                        JsonValueKind.String => type == typeof(string) ? val.GetValue<string>() : val.Deserialize(type, options),
                        JsonValueKind.Object or JsonValueKind.Array or _ => val.Deserialize(type, options)!,
                    };
                    if (deserialized?.GetType() != type) deserialized = Convert.ChangeType(deserialized, type);
                    inst.SetFieldValue(key, deserialized ?? Activator.CreateInstance(type)!);
                }
            }
            return inst;
        }

        public override void Write(Utf8JsonWriter writer, RszInstance value, JsonSerializerOptions options)
        {
            var dict = new Dictionary<string, object>(value.Fields.Length + 1);
            // NOTE: could be nice to handle dictionary arrays (KeyValuePair) as dictionaries, but not required
            dict["$type"] = value.RszClass.name;
            for (var i = 0; i < value.Fields.Length; i++) {
                var field = value.Fields[i];
                var fieldValue = value.Values[i];
                if (field.type == RszFieldType.Object && field.array) {
                    // store as structure: {"$array": "base class type", "items": []}
                    // we need it for diffs, because we might have custom conditions on the base class and not subclasses
                    dict[field.name] = new Dictionary<string, object>() {
                        { "$array", field.original_type },
                        { "items", fieldValue },
                    };
                } else if (field.type == RszFieldType.UserData) {
                    if (field.array) {
                        var list = ((IList<object>)fieldValue).Cast<RszInstance>().Select(elem => SerializeUserdata(dict, field, elem));
                        dict[field.name] = list.ToArray();
                    } else {
                        var data = SerializeUserdata(dict, field, (RszInstance)fieldValue);
                        if (data != null) {
                            dict[field.name] = data;
                        }
                    }
                } else {
                    dict[field.name] = fieldValue;
                }
            }
            JsonSerializer.Serialize(writer, dict, options);
        }

        private RszInstance DeserializeUserdata(JsonNode? val)
        {
            var userClassName = val?.AsObject()["class"]?.GetValue<string>() ?? string.Empty;
            var userClass = env.RszParser.GetRSZClass(userClassName);
            if (userClass == null) {
                throw new Exception("Could not deserialize user data reference " + val);
            }
            var path = val?.AsObject()["path"]?.GetValue<string>() ?? string.Empty;

            if (env.IsEmbeddedInstanceInfoUserdata || env.IsEmbeddedUserdata) {
                return new RszInstance(userClass, new RSZUserDataInfo_TDB_LE_67() { jsonPathHash = MurMur3HashUtils.GetHash(path) });
            } else {
                return new RszInstance(userClass, new RSZUserDataInfo() { Path = path });
            }
        }

        private Dictionary<string, string>? SerializeUserdata(Dictionary<string, object> dict, RszField field, RszInstance userRef)
        {
            if (userRef.RSZUserData == null) {
                return null;
            } else if (userRef.RSZUserData is RSZUserDataInfo curUser) {
                return new Dictionary<string, string>() {
                        { "class", curUser.ClassName ?? curUser.ReadClassName(env.RszParser) },
                        { "path", curUser.Path ?? string.Empty }
                    };
            } else if (userRef.RSZUserData is RSZUserDataInfo_TDB_LE_67 oldUser) {
                var path = oldUser.EmbeddedRSZ?.ObjectList[0].Values[0] as string;
                return new Dictionary<string, string>() {
                        { "class", oldUser.ClassName ?? oldUser.ReadClassName(env.RszParser) },
                        { "path", path ?? string.Empty }
                    };
            } else {
                throw new NotSupportedException("Unsupported rsz userdata type " + (userRef.RSZUserData?.GetType().Name ?? "NULL"));
            }
        }
    }

    public class PfbGameObjectJsonConverter : JsonConverter<PfbGameObject>
    {
        public override PfbGameObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
            if (dict == null) return null;
            var obj = new PfbGameObject();
            foreach (var (key, prop) in dict) {
                if (prop == null) continue;
                if (key == "_data") {
                    obj.Instance = prop.Deserialize<RszInstance>(options);
                } else {
                    var comp = prop.Deserialize<RszInstance>(options);
                    obj.Components.Add(comp!);
                }
            }

            return obj;
        }

        public override void Write(Utf8JsonWriter writer, PfbGameObject value, JsonSerializerOptions options)
        {
            var dict = new Dictionary<string, object>();
            dict["_data"] = JsonSerializer.SerializeToElement(value.Instance, options);
            foreach (var comp in value.Components) {
                dict[comp.RszClass.name] = JsonSerializer.SerializeToElement(comp, options);
            }
            JsonSerializer.Serialize(writer, dict, options);
        }
    }

    public class ScnGameObjectJsonConverter : JsonConverter<ScnGameObject>
    {
        public override ScnGameObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
            if (dict == null) return null;
            var obj = new ScnGameObject() { Info = new() };
            foreach (var (key, prop) in dict) {
                if (prop == null) continue;
                if (key == "_guid") {
                    if (Guid.TryParse(prop.GetValue<string>(), out var guid)) {
                        obj.Guid = guid;
                    }
                } else if (key == "_prefab") {
                    obj.Prefab ??= new();
                    obj.Prefab.Path = prop.GetValue<string>();
                } else if (key == "_data") {
                    obj.Instance = prop.Deserialize<RszInstance>(options);
                } else {
                    var comp = prop.Deserialize<RszInstance>(options);
                    obj.Components.Add(comp!);
                }
            }

            return obj;
        }

        public override void Write(Utf8JsonWriter writer, ScnGameObject value, JsonSerializerOptions options)
        {
            var dict = new Dictionary<string, object>();
            dict["_guid"] = value.Guid.ToString();
            if (!string.IsNullOrEmpty(value.Prefab?.Path)) {
                dict["_prefab"] = value.Prefab.Path;
            }
            dict["_data"] = JsonSerializer.SerializeToElement(value.Instance, options);
            foreach (var comp in value.Components) {
                dict[comp.RszClass.name] = JsonSerializer.SerializeToElement(comp, options);
            }
            JsonSerializer.Serialize(writer, dict, options);
        }
    }
}
