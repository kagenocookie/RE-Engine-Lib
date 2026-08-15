using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ReeLib.Common;
using ReeLib.Efx.Structs.Common;
using ReeLib.Efx.Structs.Pt;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Efx
{
    // note: efx version seems to consist of a uint16 and a uint8 value
    public enum EfxVersion
    {
        Unknown = 0,
        RE7      = 1179750,
        RE2      = 1769669,
        DMC5     = 1769672,
        RE3      = 2228526,
        MHRise   = 2621987,
        RE8      = 2621998,
        RERT     = 2818689,
        MHRiseSB = 2818730,
        SF6      = 3474371,
        RE4      = 3539837,
        DD2      = 4064419,
        MHWilds  = 5571972,
        RE9      = 5899767,
        Pragmata = 5965300,
        OniWS    = 5834247,
    }

    [RszGenerate, RszVersionedObject(typeof(EfxVersion))]
    public partial class EfxHeader : BaseModel
    {
        public uint magic = EfxFile.Magic;
        public int dimensionType; // 1 = 3D, 0 = 2D re7: 0,1  vfx\vfx_resource\vfx_effecteditor\efd_character_id\efd_em3600\vfx_efd_bh7_em3600_1008.efx.1179750
        public int entryCount;
        public int stringTableLength;
        public int actionCount;
        [RszVersion(EfxVersion.RE2)]
        public int fieldParameterCount;
        public int expressionParameterCount;
        public int effectGroupsCount;
        public int effectGroupsLength;
        [RszVersion(EfxVersion.RE3, EndAt = nameof(propBindingIndexCount))]
        public int boneCount;
        public int boneAttributeEntryCount;
        public int propBindingIndexCount;

        public EfxVersion Version { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            Version = (EfxVersion)handler.FileVersion;
            DefaultRead(handler);
            if (magic != EfxFile.Magic)
            {
                throw new Exception("Invalid EFX file");
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler) => DefaultWrite(handler);
    }

    public enum EfxExpressionParameterType // TODO: enum confirmed "not wrong" with dmc5 and re4, dd2, what about the rest?
    {
        /// <summary>
        /// A single float value.
        /// </summary>
        Float = 0,
        /// <summary>
        /// A single color value (stored as a 32 bit value on the first field).
        /// </summary>
        Color = 1,
        /// <summary>
        /// 3 float values - X seems to always be within the Y-Z range, so it's probably {X = InitialValue, Y = MinValue, Z = MaxValue}.
        /// </summary>
        Range = 2,
        /// <summary>
        /// Seems to always be a single float value, same as <see cref="Float"/>, though I've only found 0.0 and 1.0 cases here.
        /// </summary>
        Float2 = 3,
    }

    public class Strings : BaseModel
    {
        public string[] ExpressionParameterNames = Array.Empty<string>();
        public string[]? BoneNames;
        public string[] ActionNames = Array.Empty<string>();
        public string[] FieldParameterNames = Array.Empty<string>();
        public string[] EfxNames = Array.Empty<string>();
        public string[] GroupNames = Array.Empty<string>();

        public EfxHeader Header { get; }

        public Strings(EfxHeader header)
        {
            Header = header;
        }

        private string[] ReadStrings(int count, FileHandler handler, bool hasUnicodePairs)
        {
            var list = new string[count];
            for (int i = 0; i < count; ++i)
            {
                if (hasUnicodePairs)
                {
                    var ascii = handler.ReadAsciiString(-1, -1, false);
                    list[i] = handler.ReadWString(-1, -1, false);
                }
                else
                {
                    list[i] = handler.ReadUTF8String(-1, false);
                }
            }
            return list;
        }

        private void WriteStrings(string[]? list, FileHandler handler, bool asciiUnicodePairs)
        {
            if (list == null) return;

            foreach (var str in list)
            {
                if (asciiUnicodePairs)
                {
                    handler.WriteAsciiString(str);
                    handler.WriteWString(str);
                }
                else
                {
                    handler.WriteUTF8String(str);
                }
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            ExpressionParameterNames = ReadStrings(Header.expressionParameterCount, handler, true);
            BoneNames = ReadStrings(Header.boneCount, handler, true);
            if (Header.Version > EfxVersion.RE7) ActionNames = ReadStrings(Header.actionCount, handler, false);
            if (Header.Version > EfxVersion.RE7) FieldParameterNames = ReadStrings(Header.fieldParameterCount, handler, false);
            EfxNames = ReadStrings(Header.entryCount, handler, false);
            GroupNames = ReadStrings(Header.effectGroupsCount, handler, false);
            if (Header.Version <= EfxVersion.RE7) ActionNames = ReadStrings(Header.actionCount, handler, false);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            WriteStrings(ExpressionParameterNames, handler, true);
            WriteStrings(BoneNames, handler, true);
            if (Header.Version > EfxVersion.RE7) WriteStrings(ActionNames, handler, false);
            if (Header.Version > EfxVersion.RE7) WriteStrings(FieldParameterNames, handler, false);
            WriteStrings(EfxNames, handler, false);
            WriteStrings(GroupNames, handler, false);
            if (Header.Version <= EfxVersion.RE7) WriteStrings(ActionNames, handler, false);
            return true;
        }
    }

    public enum EfxEntryEnum
    {
        AssignToCollisionEffect = 0,
        Root = 1,
        NoAssignment = 2,
    }

    public abstract class EFXAttribute : BaseModel
    {
        public EfxAttributeType type;
        public int UniqueID;
        public EfxVersion Version;
        public bool IsTypeAttribute => type.ToString().StartsWith("Type") && this is not IExpressionAttribute and not IClipAttribute and not IMaterialExpressionAttribute;

        public static EFXAttribute Create(EfxVersion version, EfxAttributeType type, int seqNum = -1)
        {
            var item = EfxAttributeTypeRemapper.Create(type, version);
            if (item == null) throw new ArgumentException($"Unsupported EFX attribute type {type}", nameof(type));

            item.type = type;
            if (seqNum >= 0) item.UniqueID = seqNum;
            return item;
        }

        protected EFXAttribute(EfxAttributeType type)
        {
        }

        public override string ToString() => type.ToString();

        public override EFXAttribute Clone()
        {
            return this.DeepClone<EFXAttribute>();
        }
    }

    public abstract class EFXEntryBase : BaseModel
    {
        public EfxVersion Version;
        public uint nameHash;
        public string? name;
        public List<EFXAttribute> Attributes { get; } = new();

        [JsonIgnore]
        public EFXAttribute? TypeAttribute => Attributes.FirstOrDefault(attr => attr.IsTypeAttribute);

        public bool Contains(EfxAttributeType type) => Attributes.Any(attr => attr.type == type);
        public bool TryGet<T>([MaybeNullWhen(false)] out T attr) where T : EFXAttribute => (attr = Attributes.OfType<T>().FirstOrDefault()) != null;
        public void ReorderEntries() => Attributes.Sort(AttributeTypeIdComparer.Instance);

        public bool AddAttribute(EFXAttribute attr)
        {
            if (attr.IsTypeAttribute && Attributes.Any(other => other.IsTypeAttribute)) {
                Log.Error($"Entry already has a Type* attribute!");
                return false;
            }
            if (attr is IExpressionAttribute && Enum.TryParse<EfxAttributeType>(attr.type.ToString().Replace("Expression", ""), out var exprType) && !Contains(exprType)) {
                Log.Error($"Matching main attribute {attr.type.ToString().Replace("Expression", "")} not found!");
                return false;
            }
            if (attr is IMaterialExpressionAttribute && Enum.TryParse<EfxAttributeType>(attr.type.ToString().Replace("MaterialExpression", ""), out var matexprType) && !Contains(matexprType)) {
                Log.Error($"Matching main attribute {attr.type.ToString().Replace("MaterialExpression", "")} not found!");
                return false;
            }
            if (attr is IClipAttribute && Enum.TryParse<EfxAttributeType>(attr.type.ToString().Replace("MaterialClip", "").Replace("Clip", ""), out var clipType) && !Contains(clipType)) {
                Log.Error($"Matching main attribute {attr.type.ToString().Replace("MaterialClip", "").Replace("Clip", "")} not found!");
                return false;
            }
            if (attr.type is EfxAttributeType.PlayEmitter or EfxAttributeType.PlayEfx && this is EFXEntry) {
                Log.Error($"PlayEmitter and PlayEfx (probably) can't be added to the base EFX entries!");
                return false;
            }

            var newId = EfxAttributeTypeRemapper.ToAttributeTypeID(Version, attr.type);
            var nextIndex = Attributes.FindIndex(other => EfxAttributeTypeRemapper.ToAttributeTypeID(Version, other.type) >= newId);
            if (nextIndex == -1) {
                Attributes.Add(attr);
            } else {
                var higher = Attributes[nextIndex];
                if (higher.type == attr.type) {
                    Log.Error($"{attr.type} already exists!");
                    return false;
                }

                Attributes.Insert(nextIndex, attr);
            }
            return true;
        }

        public EFXAttribute? AddAttribute(EfxAttributeType key)
        {
            var attr = EFXAttribute.Create(Version, key);
            if (AddAttribute(attr)) {
                return attr;
            }

            return null;
        }

        private sealed class AttributeTypeIdComparer : IComparer<EFXAttribute>
        {
            public static readonly AttributeTypeIdComparer Instance = new();
            public int Compare(EFXAttribute? x, EFXAttribute? y)
            {
                if (x == null || y == null) return 0;
                return EfxAttributeTypeRemapper.ToAttributeTypeID(x.Version, x.type).CompareTo(EfxAttributeTypeRemapper.ToAttributeTypeID(y.Version, y.type));
            }
        }

        public override EFXEntryBase Clone()
        {
            var clone = (EFXEntryBase)Activator.CreateInstance(GetType())!;
            clone.name = name;
            clone.nameHash = nameHash;
            clone.Version = Version;
            foreach (var attr in Attributes) {
                clone.Attributes.Add((EFXAttribute)attr.Clone());
            }
            return clone;
        }
    }

    public class EfxJsonTypeResolver : DefaultJsonTypeInfoResolver
    {
        public static readonly EfxJsonTypeResolver Instance = new();

        public static readonly JsonSerializerOptions jsonOptions = new() {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            IncludeFields = true,
            IgnoreReadOnlyProperties = false,
            TypeInfoResolver = Instance,
        };

        static EfxJsonTypeResolver()
        {
            jsonOptions.Converters.Add(new BitSetJsonConverter());
            jsonOptions.Converters.Add(new EFXExpressionParameterJsonConverter());
            jsonOptions.Converters.Add(new EFXExpressionTreeJsonConverter());
            jsonOptions.Converters.Add(new EFXExpressionListJsonConverter());
            jsonOptions.Converters.Add(new MdfPropertyJsonConverter());
            jsonOptions.Converters.Add(new EFXMaterialExpressionListJsonConverter());
        }

        private sealed class EFXExpressionTreeJsonConverter : JsonConverter<EFXExpressionTree>
        {
            public override EFXExpressionTree? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject) {
                    throw new JsonException($"Expression tree should be object at {reader.TokenStartIndex}");
                }

                var (expr, list) = ReadExpression(ref reader, options);
                var parsed = EfxExpressionStringParser.Parse(expr, list);
                return parsed;
            }

            private static (string expr, List<EFXExpressionParameterName> parameters) ReadExpression(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                var expr = "";
                var parameters = new List<EFXExpressionParameterName>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        var prop = reader.GetString();
                        switch (prop) {
                            case "expression":
                                expr = reader.GetString()!;
                                break;
                            case "parameters":
                                parameters = JsonSerializer.Deserialize<List<EFXExpressionParameterName>>(ref reader, options) ?? [];
                                break;
                        }
                    }
                }

                return (expr, parameters);
            }

            public override void Write(Utf8JsonWriter writer, EFXExpressionTree value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("expression", value.root.ToString());
                writer.WritePropertyName("parameters");
                JsonSerializer.Serialize(writer, value.parameters, options);
                writer.WriteEndObject();
            }
        }

        private sealed class EFXExpressionListJsonConverter : JsonConverter<EFXExpressionList>
        {
            public override EFXExpressionList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject) {
                    throw new JsonException($"Expression list should be array at {reader.TokenStartIndex}");
                }

                var value = new EFXExpressionList();
                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName) {
                    var prop = reader.GetString();
                    switch (prop) {
                        case "version":
                            value.Version = JsonSerializer.Deserialize<EfxVersion>(ref reader, options);
                            break;
                        case "parsedExpressions":
                            value.ParsedExpressions = JsonSerializer.Deserialize<List<EFXExpressionTree>>(ref reader, options);
                            break;
                        case "expressions":
                            value.expressions = JsonSerializer.Deserialize<List<EFXExpressionObject>>(ref reader, options) ?? [];
                            break;
                    }
                }

                // Q: would we wanna fully reconstruct expressions from parsed here?
                // foreach (var p in value.ParsedExpressions ?? []) {
                //     var exp = new EFXExpressionObject(value.Version);
                //     // EfxExpressionTreeUtils.FlattenExpressions(exp.components, p, efx); // TODO
                //     value.expressions.Add(exp);
                // }

                return value;
            }

            public override void Write(Utf8JsonWriter writer, EFXExpressionList value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("version");
                JsonSerializer.Serialize(writer, value.Version, options);
                writer.WritePropertyName("parsedExpressions");
                JsonSerializer.Serialize(writer, value.ParsedExpressions, options);
                writer.WritePropertyName("expressions");
                JsonSerializer.Serialize(writer, value.expressions, options);
                writer.WriteEndObject();
            }
        }

        private sealed class EFXMaterialExpressionListJsonConverter : JsonConverter<EFXMaterialExpressionList>
        {
            public override EFXMaterialExpressionList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject) {
                    throw new JsonException($"Expression list should be array at {reader.TokenStartIndex}");
                }

                var value = new EFXMaterialExpressionList();
                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName) {
                    var prop = reader.GetString();
                    switch (prop) {
                        case "version":
                            value.Version = JsonSerializer.Deserialize<EfxVersion>(ref reader, options);
                            break;
                        case "parsedExpressions":
                            value.ParsedExpressions = JsonSerializer.Deserialize<List<EFXExpressionTree>>(ref reader, options);
                            break;
                        case "expressions":
                            value.expressions = JsonSerializer.Deserialize<List<EFXMaterialExpression>>(ref reader, options) ?? [];
                            break;
                        case "indices":
                            value.indices = JsonSerializer.Deserialize<uint[]>(ref reader, options) ?? [];
                            break;
                    }
                }

                return value;
            }

            public override void Write(Utf8JsonWriter writer, EFXMaterialExpressionList value, JsonSerializerOptions options)
            {
                if (value.ParsedExpressions == null) {
                    writer.WriteNullValue();
                    return;
                }

                writer.WriteStartObject();
                writer.WritePropertyName("version");
                JsonSerializer.Serialize(writer, value.Version, options);
                writer.WritePropertyName("parsedExpressions");
                JsonSerializer.Serialize(writer, value.ParsedExpressions, options);
                writer.WritePropertyName("indices");
                JsonSerializer.Serialize(writer, value.indices, options);
                writer.WritePropertyName("expressions");
                JsonSerializer.Serialize(writer, value.expressions, options);
                writer.WriteEndObject();
            }
        }

        private sealed class EFXExpressionParameterJsonConverter : JsonConverter<EFXExpressionParameter>
        {
            public override EFXExpressionParameter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject) {
                    throw new JsonException($"Expression parameter should be object at {reader.TokenStartIndex}");
                }

                var res = new EFXExpressionParameter();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        var prop = reader.GetString();
                        reader.Read();
                        switch (prop) {
                            case "type":
                                res.type = Enum.Parse<EfxExpressionParameterType>(reader.GetString()!);
                                break;
                            case "name":
                                res.name = reader.GetString();
                                res.expressionParameterNameUTF8Hash = MurMur3HashUtils.GetUTF8Hash(res.name!);
                                res.expressionParameterNameUTF16Hash = MurMur3HashUtils.GetHash(res.name!);
                                break;
                            case "value":
                                switch (res.type) {
                                    case EfxExpressionParameterType.Float:
                                        res.value1 = reader.GetSingle();
                                        break;
                                    case EfxExpressionParameterType.Float2:
                                        res.Float2 = JsonSerializer.Deserialize<Vector2>(ref reader, options);
                                        break;
                                    case EfxExpressionParameterType.Color:
                                        res.Color = JsonSerializer.Deserialize<Color>(ref reader, options);
                                        break;
                                    case EfxExpressionParameterType.Range:
                                        res.Range = JsonSerializer.Deserialize<Vector3>(ref reader, options);
                                        break;
                                }
                                break;
                        }
                    }
                }
                return res;
            }

            public override void Write(Utf8JsonWriter writer, EFXExpressionParameter value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("type", value.type.ToString());
                writer.WriteString("name", value.name ?? "");
                writer.WritePropertyName("value");
                JsonSerializer.Serialize(writer, value.ValueObject, options);
                writer.WriteEndObject();
            }
        }

        private sealed class MdfPropertyJsonConverter : JsonConverter<MdfProperty>
        {
            public override MdfProperty? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject) {
                    throw new JsonException($"Expression parameter should be object at {reader.TokenStartIndex}");
                }

                var res = new MdfProperty();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        var prop = reader.GetString();
                        reader.Read();
                        switch (prop) {
                            case nameof(MdfProperty.Version):
                                res.Version = (EfxVersion)reader.GetInt32();
                                break;
                            case nameof(MdfProperty.parameterType):
                                res.parameterType = Enum.Parse<MaterialParameterType>(reader.GetString()!);
                                break;
                            case nameof(MdfProperty.PropertyNameUTF8Hash):
                                res.PropertyNameUTF8Hash = reader.GetUInt32();
                                break;
                            case nameof(MdfProperty.mdfPropertyIndex):
                                res.mdfPropertyIndex = reader.GetInt32();
                                break;
                            case nameof(MdfProperty.mdfParameterValueCount):
                                res.mdfParameterValueCount = reader.GetUInt16();
                                break;
                            case nameof(MdfProperty.flags):
                                res.flags = reader.GetInt32();
                                break;
                            case "value":
                                if (res.parameterType == MaterialParameterType.Texture) {
                                    res.TextureValue = JsonSerializer.Deserialize<MdfPropertyTextureValue>(ref reader, options);
                                } else {
                                    res.VectorValue = JsonSerializer.Deserialize<Vector4>(ref reader, options);
                                }
                                break;
                            case nameof(MdfProperty.texturePath):
                                res.texturePath = reader.GetString();
                                break;
                        }
                    }
                }
                return res;
            }

            public override void Write(Utf8JsonWriter writer, MdfProperty value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber(nameof(MdfProperty.Version), (int)value.Version);
                writer.WriteString(nameof(MdfProperty.parameterType), value.parameterType.ToString());
                writer.WriteNumber(nameof(MdfProperty.PropertyNameUTF8Hash), value.PropertyNameUTF8Hash);
                writer.WriteNumber(nameof(MdfProperty.mdfPropertyIndex), value.mdfPropertyIndex);
                writer.WriteNumber(nameof(MdfProperty.mdfParameterValueCount), value.mdfParameterValueCount);
                writer.WriteNumber(nameof(MdfProperty.flags), value.flags);
                if (value.parameterType == MaterialParameterType.Texture) {
                    writer.WritePropertyName("value");
                    JsonSerializer.Serialize(writer, value.TextureValue, options);
                    writer.WriteString(nameof(MdfProperty.texturePath), value.texturePath);
                } else {
                    writer.WritePropertyName("value");
                    JsonSerializer.Serialize(writer, value.VectorValue, options);
                }
                writer.WriteEndObject();
            }
        }

        private sealed class BitSetJsonConverter : JsonConverter<BitSet>
        {
            public override BitSet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject) {
                    throw new JsonException($"Expression parameter should be object at {reader.TokenStartIndex}");
                }

                BitSet? set = null;
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        var prop = reader.GetString();
                        reader.Read();
                        switch (prop) {
                            case "bitCount":
                                set = new BitSet(reader.GetInt32());
                                break;
                            case "bitNames":
                                var names = JsonSerializer.Deserialize<string[]>(ref reader, options) ?? [];
                                set = new BitSet(set?.BitCount ?? names.Length) { BitNameDict = names.Select((n, i) => (n, i)).ToDictionary(kv => kv.i, kv => kv.n) };
                                break;
                            case "bits":
                                var setBits = JsonSerializer.Deserialize<int[]>(ref reader, options);
                                foreach (var b in setBits ?? []) {
                                    set!.SetBit(b, true);
                                }
                                break;
                        }
                    }
                }

                return set;
            }

            public override void Write(Utf8JsonWriter writer, BitSet value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("bitCount", value.BitCount);
                writer.WritePropertyName("bitNames");
                JsonSerializer.Serialize(writer, value.BitNames, options);
                writer.WritePropertyName("bits");
                writer.WriteStartArray();
                for (int i = 0; i < value.BitCount; i++) {
                    if (value.HasBit(i)) {
                        writer.WriteNumberValue(i);
                    }
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);
            Debug.Assert(type == jsonTypeInfo.Type);

            if (type == typeof(EFXAttribute))
            {
                var subtypes = typeof(EFXAttribute).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(EFXAttribute)) && t != typeof(EFXAttribute));

                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "$type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes = {}
                };
                foreach (var t in subtypes) {
                    jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(t, t.FullName!));
                }
                jsonTypeInfo.PreferredPropertyObjectCreationHandling = JsonObjectCreationHandling.Populate;
            }
            else if (type == typeof(EfxFile) || type == typeof(EFXEntry) || type == typeof(EFXAction))
            {
                jsonTypeInfo.PreferredPropertyObjectCreationHandling = JsonObjectCreationHandling.Populate;
            }
            else if (type == typeof(EFXExpressionDataBase))
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes = {}
                };
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(EFXExpressionDataFloat), (int)ExpressionComponentStorageType.Float));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(EFXExpressionDataBinaryOperator), (int)ExpressionComponentStorageType.BinaryOperator));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(EFXExpressionDataUnaryOperator), (int)ExpressionComponentStorageType.UnaryOperator));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(EFXExpressionDataFunction), (int)ExpressionComponentStorageType.Function));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(EFXExpressionDataParameterHash), (int)ExpressionComponentStorageType.ParameterHash));
                jsonTypeInfo.PreferredPropertyObjectCreationHandling = JsonObjectCreationHandling.Populate;
            }
            else if (type == typeof(PtBehaviorVariableDataBase))
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "$type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes = {}
                };
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableDataColor), PtBehaviorPropType.PropColor.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableFloat), PtBehaviorPropType.PropFloat.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableFloat2), PtBehaviorPropType.PropFloat2.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableFloat3), PtBehaviorPropType.PropFloat3.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableInteger), PtBehaviorPropType.PropInt.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableEnum), PtBehaviorPropType.PropEnum.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableDataWString), PtBehaviorPropType.PropWstringName.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableDataPrefabPath), PtBehaviorPropType.PropPrefabpath.ToString()));
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(PtBehaviorVariableDataPrefabUnknown), "_unknown"));
                jsonTypeInfo.PreferredPropertyObjectCreationHandling = JsonObjectCreationHandling.Populate;
            }

            return jsonTypeInfo;
        }
    }

    public class EFXEntry : EFXEntryBase
    {
        public int index;
        public EfxEntryEnum entryAssignment;
        public List<string> Groups { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref index);
            handler.Read(ref nameHash);
            handler.Read(ref entryAssignment);
            var attributeCount = handler.Read<int>();
            int lastAttributeTypeId = -1;
            for (int i = 0; i < attributeCount; ++i) {
                var typeId = handler.Read<int>();
                DataInterpretationException.DebugWarnIf(typeId < lastAttributeTypeId, $"EFX attribute ID {typeId} is out of order from previous {lastAttributeTypeId}");
                var type = Version.GetAttributeType(lastAttributeTypeId = typeId);
                int expectedSize = -1;
                if (Version >= EfxVersion.MHWilds) {
                    handler.Read(ref expectedSize);
                }
                var seqNum = handler.Read<int>();
                var attr = EFXAttribute.Create(Version, type, seqNum);
                attr.Version = Version;
                attr.Read(handler);
                // Log.Debug($"Read {attr.type} at {attr.Start}");
                if (expectedSize != -1 && expectedSize - 4 != attr.StructSize) {//UniqueID is included in the struct size, so subtract 4
                    throw new Exception($"EFX attribute ({attr.type}) was not properly read. Expected: {expectedSize} Actual: {attr.StructSize+4} Start:{attr.Start} End:{attr.Start + attr.StructSize}");
                }
                Attributes.Add(attr);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref index);
            nameHash = MurMur3HashUtils.GetUTF8Hash(name ?? string.Empty);
            handler.Write(ref nameHash);
            handler.Write(ref entryAssignment);
            handler.Write(Attributes.Count);
            foreach (var attr in Attributes) {
                handler.Write(Version.ToAttributeTypeID(attr.type));
                var sizeOffset = handler.Tell();
                if (Version >= EfxVersion.MHWilds) {
                    handler.Skip(sizeof(int));
                }

                handler.Write(attr.UniqueID);

                attr.Write(handler);
                if (Version >= EfxVersion.MHWilds) {
                    handler.Write(sizeOffset, (int)(attr.StructSize + 4));
                }
            }
            return true;
        }

        public override EFXEntry Clone()
        {
            var clone = (EFXEntry)base.Clone();
            clone.Groups.Clear();
            clone.Groups.AddRange(Groups);
            clone.entryAssignment = entryAssignment;
            clone.index = index;
            return clone;
        }

        public override string ToString() => name ?? $"Entry {index}";
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class EFXExpressionParameter : BaseModel
    {
        public uint expressionParameterNameUTF16Hash;
        public uint expressionParameterNameUTF8Hash;
        public EfxExpressionParameterType type;
        public float value1;
        public float value2;
        public float value3;
        [RszIgnore] public string? name;

        public Vector2 Float2
        {
            get => type == EfxExpressionParameterType.Float2 ? new Vector2(value1, value2) : throw new Exception("Expression parameter is not a float2");
            set {
                type = EfxExpressionParameterType.Float2;
                value1 = value.X;
                value2 = value.Y;
            }
        }

        public via.Color Color
        {
            get => type == EfxExpressionParameterType.Color ? new via.Color() { rgba = (uint)BitConverter.SingleToInt32Bits(value1) } : throw new Exception("Expression parameter is not a color");
            set {
                type = EfxExpressionParameterType.Color;
                value1 = BitConverter.Int32BitsToSingle((int)value.rgba);
            }
        }

        public Vector3 Range
        {
            get => type == EfxExpressionParameterType.Range ? new Vector3(value1, value2, value3) : throw new Exception("Expression parameter is not a range");
            set {
                type = EfxExpressionParameterType.Range;
                value1 = value.X;
                value2 = value.Y;
                value3 = value.Z;
            }
        }

        public object? ValueObject
        {
            get => type switch {
                EfxExpressionParameterType.Float => value1,
                EfxExpressionParameterType.Float2 => Float2,
                EfxExpressionParameterType.Color => Color,
                EfxExpressionParameterType.Range => Range,
                _ => Range,
            };
        }

        public override string ToString() => type switch {
            EfxExpressionParameterType.Color => $"{type}  {Color}",
            EfxExpressionParameterType.Float => value1.ToString(),
            EfxExpressionParameterType.Float2 => $"{type}  {Float2}",
            EfxExpressionParameterType.Range => $"{type}  {Range}",
            _ => $"{type}  {Range}",
        };
    }

    internal struct EFXBoneNameValuePair
    {
        public uint nameHash;
        public uint value;
    }

    public class EFXAction : EFXEntryBase
    {
        public int actionUnkn0;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref actionUnkn0);
            handler.Read(ref nameHash);
            var actionAttributeCount = handler.Read<int>();
            for (int i = 0; i < actionAttributeCount; ++i) {
                var type = Version.GetAttributeType(handler.Read<int>());
                var expectedSize = Version >= EfxVersion.MHWilds ? handler.Read<int>() : -1;
                var seqNum = handler.Read<int>();
                var attr = EFXAttribute.Create(Version, type, seqNum);
                attr.Version = Version;
                attr.Read(handler);
                if (expectedSize != -1 && expectedSize - 4 != attr.StructSize) {//UniqueID is included in the struct size, so subtract 4
                    throw new Exception($"EFX attribute ({attr.type}) was not properly read. Expected: {expectedSize} Actual: {attr.StructSize+4} Start:{attr.Start} End:{attr.Start + attr.StructSize}");
                }
                Attributes.Add(attr);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            nameHash = MurMur3HashUtils.GetUTF8Hash(name ?? string.Empty);

            handler.Write(ref actionUnkn0);
            handler.Write(ref nameHash);
            handler.Write(Attributes.Count);
            foreach (var attr in Attributes) {
                handler.Write(Version.ToAttributeTypeID(attr.type));
                var sizeOffset = handler.Tell();
                if (Version >= EfxVersion.MHWilds) {
                    handler.Skip(4);
                }
                handler.Write(attr.UniqueID);
                attr.Write(handler);
                if (Version >= EfxVersion.MHWilds) {
                    handler.Write(sizeOffset, (int)(attr.StructSize + 4));
                }
            }
            return true;
        }

        public override EFXAction Clone()
        {
            var clone = (EFXAction)base.Clone();
            clone.actionUnkn0 = actionUnkn0;
            return clone;
        }

        public override string ToString() => name ?? $"Action {actionUnkn0}";
    }

    [RszGenerate]
    public partial class EFXFieldParameterValue : BaseModel
    {
        [RszIgnore] public EfxVersion Version;

        public uint unkn0;
        public uint fieldParameterNameHash;
        public uint unkn2;
        public uint type;
        public uint unkn4;
        public int value_ukn1;
        [RszIgnore] public uint value_ukn2;
        [RszIgnore] public uint value_ukn3;
        [RszIgnore] public float value_ukn4;
        [RszIgnore] public float value_ukn5;
        [RszIgnore] public float value_ukn6;
        [RszIgnore] public float wilds_unkn0;
        [RszIgnore] public string? name;
        [RszIgnore] public string? filePath;

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            if (type == 196) {
                filePath = handler.ReadWString(-1, value_ukn1, false);
            } else if (Version > EfxVersion.RE7) {
                handler.Read(ref value_ukn2);
                handler.Read(ref value_ukn3);
                handler.Read(ref value_ukn4);
                handler.Read(ref value_ukn5);
                handler.Read(ref value_ukn6);
                if (Version >= EfxVersion.MHWilds) {
                    handler.Read(ref wilds_unkn0);
                }
                if (type is 110 or 144 or 183 or 184 or 202 or 194 or 215 or 217) {
                    filePath = handler.ReadWString(-1, handler.Read<int>(), false);
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (type == 196) {
                filePath ??= string.Empty;
                value_ukn1 = filePath.Length + 2;
                DefaultWrite(handler);
                handler.WriteWString(filePath);
            } else {
                DefaultWrite(handler);
                handler.Write(ref value_ukn2);
                handler.Write(ref value_ukn3);
                handler.Write(ref value_ukn4);
                handler.Write(ref value_ukn5);
                handler.Write(ref value_ukn6);
                if (Version >= EfxVersion.MHWilds) {
                    handler.Write(ref wilds_unkn0);
                }
                if (type is 110 or 144 or 183 or 184 or 202 or 194 or 215 or 217) {
                    filePath ??= "";
                    handler.Write(filePath.Length + 1);
                    handler.WriteWString(filePath);
                }
            }
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class EffectGroup : BaseModel
    {
        [RszStringHash(nameof(groupName))] public uint conditionalEffectGroupNameHashUTF16;
        [RszStringUTF8Hash(nameof(groupName))] public uint conditionalEffectGroupNameHashUTF8;
        [RszArraySizeField(nameof(efxEntryIndexes))] public int valueCount;
        [RszFixedSizeArray(nameof(valueCount))] public int[]? efxEntryIndexes;
        [RszIgnore] public string groupName = string.Empty;

        public override string ToString() => groupName;
    }

    public class EFXBone
    {
        public string name = string.Empty;
        public uint value;

        public override string ToString() => $"{name} = {value}";
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class EFXUvarGroup : BaseModel
    {
        [RszIgnore] public int uvarType;

        [RszInlineWString(ByteSize = true), RszConditional(nameof(uvarType), "==", 2)] public string? path;
        [RszInlineWString(ByteSize = true), RszConditional(nameof(uvarType), "==", 2)] public string? group;

        public override string ToString() => $"[{uvarType}] {group} {path}";
    }

    public interface IBoneRelationAttribute
    {
        string? ParentBone { get; set; }
    }

    public interface IExpressionAttribute
    {
        EFXExpressionList? Expression { get; set; }
        BitSet ExpressionBits { get; }
    }

    public interface IMaterialExpressionAttribute
    {
        EFXMaterialExpressionList? MaterialExpressions { get; set; }
    }

    public interface IExpressionParameterSource
    {
        EFXExpressionParameter? FindParameterByHash(uint hash);
    }

    public interface IClipAttribute
    {
        EfxClipData Clip { get; }
        BitSet ClipBits { get; }
    }

    public interface IMaterialClipAttribute : IClipAttribute
    {
        EfxMaterialClipData MaterialClip { get; }
        EfxClipData IClipAttribute.Clip => MaterialClip;
    }

    public static class EfxExtensions
    {
        public static EFXExpressionParameterName? GetParameterByHash(this IEnumerable<EFXExpressionParameterName> list, uint hash)
        {
            foreach (var p in list) {
                if (p.parameterNameHash == hash) return p;
            }
            return null;
        }
    }
}

namespace ReeLib
{
    using ReeLib.Efx;
    using ReeLib.Efx.Structs.Basic;

    public partial class EfxFile : BaseFile, IExpressionParameterSource, ICloneable, ITargetCloneable<EfxFile>
    {
        public List<EFXEntry> Entries { get; } = new();
        public List<EFXBone> Bones { get; } = new();

        public EfxHeader Header { get; } = new EfxHeader();
        public Strings? Strings;

        public List<short> BoneRelations { get; } = new();
        public List<EFXExpressionParameter> ExpressionParameters { get; } = new();
        public List<EFXAction> Actions { get; } = new();
        public List<EFXFieldParameterValue> FieldParameterValues { get; } = new();
        public List<EffectGroup> EffectGroups { get; } = new();
        public List<EFXUvarGroup> UvarGroups { get; } = new();

        [JsonIgnore]
        public EfxFile? parentFile;

        public const uint Magic = 0x72786665;

        public EfxFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        [JsonConstructor]
        private EfxFile() : base(new FileHandler()) { }

        public static EfxVersion[] AllVersions => (EfxVersion[])Enum.GetValues(typeof(EfxVersion));

        object ICloneable.Clone()
        {
            return CloneTo(new EfxFile(new FileHandler()));
        }

        public EfxFile CloneTo(EfxFile c)
        {
            Header.CloneFieldsTo(c.Header);
            Entries.CloneListTo(c.Entries);
            Bones.CloneListTo(c.Bones);
            Strings?.CloneFieldsTo(c.Strings = new(c.Header));
            c.BoneRelations.AddRange(BoneRelations);
            ExpressionParameters.CloneListTo(c.ExpressionParameters);
            Actions.CloneListTo(c.Actions);
            FieldParameterValues.CloneListTo(c.FieldParameterValues);
            EffectGroups.CloneListTo(c.EffectGroups);
            UvarGroups.CloneListTo(c.UvarGroups);
            foreach (var sub in c.GetEmbeddedFiles()) {
                if (sub != c) {
                    sub.parentFile = c;
                    sub.FileHandler = FileHandler;
                }
            }
            return c;
        }

        public void Clear()
        {
            ExpressionParameters.Clear();
            FieldParameterValues.Clear();
            BoneRelations.Clear();
            Actions.Clear();
            Entries.Clear();
            Bones.Clear();
            UvarGroups.Clear();
            EffectGroups.Clear();
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            Strings = new Strings(Header);
            Strings.Read(handler);

            Clear();
            handler.Seek(Strings.Start + Header.stringTableLength);

            for (int i = 0; i < Header.expressionParameterCount; ++i) {
                var param = new EFXExpressionParameter();
                param.name = Strings.ExpressionParameterNames[i];
                param.Read(handler);
                ExpressionParameters.Add(param);
            }

            for (int i = 0; i < Header.boneCount; ++i) {
                var data = handler.Read<EFXBoneNameValuePair>();
                var boneName = Strings.BoneNames![i];
                Bones.Add(new EFXBone() {
                    name = boneName,
                    value = data.value,
                });
            }
            for (int i = 0; i < Header.boneAttributeEntryCount; ++i) BoneRelations.Add(handler.Read<short>());

            if (Header.Version > EfxVersion.RE7) {
                ReadActions(handler);
            }

            for (int i = 0; i < Header.fieldParameterCount; ++i) {
                var param = new EFXFieldParameterValue();
                param.Version = Header.Version;
                param.name = Strings.FieldParameterNames[i];
                param.Read(handler);
                FieldParameterValues.Add(param);
            }
            if (Header.Version <= EfxVersion.RE7) {
                ReadActions(handler);
            }

            for (int i = 0; i < Header.entryCount; ++i) {
                var entry = new EFXEntry() { Version = Header.Version };
                entry.name = Strings.EfxNames[i];
                entry.Read(handler);
                Entries.Add(entry);
            }

            for (int i = 0; i < Header.effectGroupsCount; ++i) {
                var effect = new EffectGroup();
                effect.groupName = Strings.GroupNames[i];
                effect.Read(handler);
                EffectGroups.Add(effect);

                if (effect.efxEntryIndexes == null) continue;
                foreach (var index in effect.efxEntryIndexes) {
                    Entries[index].Groups.Add(effect.groupName);
                }
            }

            if (Header.Version > EfxVersion.DMC5) {
                var uvarType1 = handler.Read<int>();
                var uvarType2 = handler.Read<int>();
                // note: found these as either 0 or 1 in DD2
                // always 0 for RE4,DMC5,RERT
                if (uvarType1 != 0) {
                    var grp = new EFXUvarGroup() { uvarType = uvarType1 };
                    grp.Read(handler);
                    UvarGroups.Add(grp);
                }
                if (uvarType2 != 0) {
                    var grp = new EFXUvarGroup() { uvarType = uvarType2 };
                    grp.Read(handler);
                    UvarGroups.Add(grp);
                }
                if (uvarType1 > 2 || uvarType2 > 2) {
                    throw new Exception("Found unhandled uvar type? " + uvarType1 + " /" + uvarType2);
                }
            }

            foreach (var action in Actions) {
                foreach (var a in action.Attributes.OfType<EFXAttributePlayEmitter>()) {
                    if (a.efxrData != null) {
                        a.efxrData.parentFile = this;
                    }
                }
            }
            if (Header.Version > EfxVersion.DMC5)
            {
                SetupBoneReferences();
            }

            return true;
        }

        public void ParseExpressions()
        {
            foreach (var entry in Entries) {
                foreach (var attr in entry.Attributes) {
                    if (attr is IExpressionAttribute expr && expr.Expression != null) {
                        expr.Expression.ParsedExpressions = ParseExpressions(expr.Expression);
                    }
                    if (attr is IMaterialExpressionAttribute expr2 && expr2.MaterialExpressions != null) {
                        expr2.MaterialExpressions.ParsedExpressions = ParseExpressions(expr2.MaterialExpressions);
                    }
                }
            }

            foreach (var action in Actions) {
                foreach (var a in action.Attributes.OfType<EFXAttributePlayEmitter>()) {
                    if (a.efxrData != null) {
                        a.efxrData.ParseExpressions();
                    }
                }
            }
        }

        public List<EFXExpressionTree> ParseExpressions(EFXExpressionContainer container)
            => EfxExpressionTreeUtils.ReconstructExpressionTreeList(container.Expressions, this);

        public void FlattenExpressionTrees(EFXExpressionContainer expression)
        {
            if (expression.ParsedExpressions == null) return;

            foreach (var expr in expression.ParsedExpressions) {
                var target = FlattenExpressionTree(expr);
                expression.AddExpression(target);
            }
        }

        public EFXExpressionObject FlattenExpressionTree(EFXExpressionTree tree)
        {
            var target = new EFXExpressionObject();
            if (tree.root == ExpressionAtom.Null) return target;
            target.components ??= new();
            EfxExpressionTreeUtils.FlattenExpressions(target.components, tree, this);
            target.parameters = tree.parameters.ToList();
            return target;
        }

        private void ReadActions(FileHandler handler)
        {
            for (int i = 0; i < Header!.actionCount; ++i) {
                var action = new EFXAction() { Version = Header.Version };
                action.name = Strings!.ActionNames[i];
                action.Read(handler);
                foreach (var a in action.Attributes.OfType<EFXAttributePlayEmitter>()) {
                    if (a.efxrData != null) {
                        a.efxrData.parentFile = this;
                    }
                }
                Actions.Add(action);
            }
        }

        private void SetupBoneReferences()
        {
            var bones = parentFile?.Bones ?? Bones;
            if (bones.Count == 0) return;

            int index = 0;
            foreach (var entry in Entries) {
                foreach (var attr in entry.Attributes) {
                    if (attr is IBoneRelationAttribute parented) {
                        short parentBoneIndex;
                        if (index >= BoneRelations.Count)
                        {
                            Log.Warn($"EFX entry[{Entries.IndexOf(entry)}] {entry}.{attr} has too many bone relations ({index+1} > {BoneRelations.Count}), attaching to root bone {(Bones.FirstOrDefault()?.name)}");
                            parentBoneIndex = 0;
                        }
                        else
                        {
                            parentBoneIndex = BoneRelations[index++];
                        }

                        if (parentBoneIndex >= 0 && parentBoneIndex < bones.Count)
                        {
                            parented.ParentBone = bones[parentBoneIndex].name;
                        }
                        else
                        {
                            parented.ParentBone = null;
                            if (parentBoneIndex != -1) Log.Warn($"Invalid EFX parent bone index {parentBoneIndex} for relation {index - 1}");
                        }
                    }
                }
            }

            foreach (var action in Actions) {
                foreach (var a in action.Attributes.OfType<EFXAttributePlayEmitter>()) {
                    a.efxrData?.SetupBoneReferences();
                }
            }
        }

        private void UpdateEffectGroups()
        {
            var dict = new Dictionary<string, List<int>>();
            for (var i = 0; i < Entries.Count; i++)
            {
                foreach (var grp in Entries[i].Groups)
                {
                    if (!dict.TryGetValue(grp, out var ids)) {
                        dict[grp] = ids = new List<int>();
                    }

                    ids.Add(i);
                }
            }

            foreach (var grp in EffectGroups) {
                if (dict.Remove(grp.groupName, out var data)) {
                    grp.efxEntryIndexes = data.ToArray();
                } else {
                    grp.efxEntryIndexes = [];
                }
            }

            foreach (var unaccounted in dict) {
                EffectGroups.Add(new EffectGroup() {
                    groupName = unaccounted.Key,
                    valueCount = unaccounted.Value.Count,
                    efxEntryIndexes = unaccounted.Value.ToArray(),
                    conditionalEffectGroupNameHashUTF16 = MurMur3HashUtils.GetHash(unaccounted.Key),
                    conditionalEffectGroupNameHashUTF8 = MurMur3HashUtils.GetUTF8Hash(unaccounted.Key)
                });
            }
        }

        private void UpdateHeaderData(EfxVersion version)
        {
            if (Strings == null || Header.Version != version || Strings.Header != Header) {
                Header.Version = version;
                Strings = new(Header);
            }

            Header.expressionParameterCount = ExpressionParameters.Count;
            Header.boneCount = Bones.Count;
            Header.entryCount = Entries.Count;
            Header.effectGroupsCount = EffectGroups.Count;
            Header.actionCount = Actions.Count;
            Header.fieldParameterCount = FieldParameterValues.Count;
            Header.boneAttributeEntryCount = Bones.Count;

            Strings.EfxNames = Entries.Select(e => e.name ?? string.Empty).ToArray();
            Strings.GroupNames = EffectGroups.Select(e => e.groupName ?? string.Empty).ToArray();
            Strings.BoneNames = Bones.Select(b => b.name ?? string.Empty).ToArray();
            Strings.ActionNames = Actions.Select(a => a.name ?? string.Empty).ToArray();
            Strings.FieldParameterNames = FieldParameterValues.Select(a => a.name ?? string.Empty).ToArray();
            Strings.ExpressionParameterNames = ExpressionParameters.Select(a => a.name ?? string.Empty).ToArray();
        }

        protected override bool DoWrite()
        {
            if (Header == null) return false;
            var handler = FileHandler;

            UpdateEffectGroups();
            UpdateHeaderData(Header.Version);

            Header.Write(handler);
            long writeStart = handler.Tell();
            Strings!.Write(handler);
            Header.stringTableLength = (int)(handler.Tell() - writeStart);

            foreach (var exprParam in ExpressionParameters) {
                exprParam.expressionParameterNameUTF16Hash = MurMur3HashUtils.GetHash(exprParam.name ?? string.Empty);
                exprParam.expressionParameterNameUTF8Hash = MurMur3HashUtils.GetUTF8Hash(exprParam.name ?? string.Empty);
                exprParam.Write(handler);
            }

            if (Header.Version > EfxVersion.DMC5) {
                foreach (var bone in Bones) {
                    var pair = new EFXBoneNameValuePair() {
                        nameHash = MurMur3HashUtils.GetHash(bone.name),
                        value = bone.value,
                    };
                    handler.Write(ref pair);
                }

                Header.boneAttributeEntryCount = 0;
                foreach (var entry in Entries) {
                    foreach (var attr in entry.Attributes) {
                        if (attr is IBoneRelationAttribute parented) {
                            var index = string.IsNullOrEmpty(parented.ParentBone) ? -1 : Bones.FindIndex(b => b.name == parented.ParentBone);
                            handler.Write((short)index);
                            Header.boneAttributeEntryCount++;
                        }
                    }
                }
            }

            Actions.Write(handler);

            FieldParameterValues.Write(handler);
            Entries.Write(handler);

            writeStart = handler.Tell();
            EffectGroups.Write(handler);
            Header.effectGroupsLength = (int)(handler.Tell() - writeStart);

            if (Header.Version > EfxVersion.DMC5) {
                handler.Write(UvarGroups.FirstOrDefault()?.uvarType ?? 0);
                handler.Write(UvarGroups.Skip(1).FirstOrDefault()?.uvarType ?? 0);
                if (UvarGroups.Count >= 1)
                {
                    UvarGroups[0].Write(handler);
                }
                if (UvarGroups.Count >= 2)
                {
                    UvarGroups[1].Write(handler);
                }
            }

            var endPosition = handler.Tell();
            handler.Seek(0);

            // write header again to update the length params
            Header.Write(handler);
            handler.Seek(endPosition);
            return true;
        }

        public EFXExpressionParameter? FindParameterByHash(uint hash)
        {
            foreach (var p in (parentFile ?? this).ExpressionParameters) {
                if (p.expressionParameterNameUTF8Hash == hash) return p;
            }
            return null;
        }
    }
}
