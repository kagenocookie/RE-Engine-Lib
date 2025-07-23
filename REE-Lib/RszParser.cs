using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReeLib.Common;

namespace ReeLib
{
    /// <summary>
    /// Rsz json parser
    /// </summary>
    public class RszParser
    {
        private static readonly Dictionary<string, RszParser> instanceDict = new();

        public static RszParser GetInstance(string jsonPath)
        {
            if (!instanceDict.TryGetValue(jsonPath, out var rszParser))
            {
                rszParser = new RszParser(jsonPath);
                instanceDict[jsonPath] = rszParser;
            }
            return rszParser;
        }

        private readonly Dictionary<uint, RszClass> classDict;
        private readonly Dictionary<string, RszClass> classNameDict;

        public Dictionary<uint, RszClass> ClassDict => classDict;
        public string[]? nonGenericClassNames;
        public string[] NonGenericClassNames
        {
            get
            {
                if (nonGenericClassNames == null)
                {
                    List<string> list = new();
                    foreach (var name in classNameDict.Keys)
                    {
                        if (name.Length > 0 && !name.Contains('<') && !name.Contains('`') && !name.Contains('[') && !name.Contains('!'))
                        {
                            list.Add(name);
                        }
                    }
                    list.Sort();
                    nonGenericClassNames = list.ToArray();
                }
                return nonGenericClassNames;
            }
        }

        public RszParser(string jsonPath)
        {
            classDict = new();
            classNameDict = new();
            using FileStream fileStream = File.OpenRead(jsonPath);
            var dict = JsonSerializer.Deserialize<Dictionary<string, RszClass>>(fileStream);
            if (dict != null)
            {
                var unresolvedStructs = new HashSet<RszField>();
                foreach (var item in dict)
                {
                    var value = item.Value;
                    if (!uint.TryParse(item.Key, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var typeId)) {
                        continue;
                    }
                    value.typeId = typeId;
                    classDict[value.typeId] = value;
                    classNameDict[value.name] = value;

                    foreach (var field in value.fields)
                    {
                        if (field.type == RszFieldType.Data)
                        {
                            field.GuessDataType();
                        }

                        if (field.type == RszFieldType.Struct)
                        {
                            if (classNameDict.TryGetValue(field.original_type, out var structCls))
                            {
                                field.StructClass = structCls;
                            }
                            else
                            {
                                unresolvedStructs.Add(field);
                            }
                        }
                    }
                }
                foreach (var structField in unresolvedStructs)
                {
                    structField.StructClass = classNameDict[structField.original_type];
                }

                string patchJsonPath = Path.Combine(
                    "Data", "RszPatch",
                    Path.GetFileNameWithoutExtension(jsonPath) + "_patch.json"); ;
                ReadPatch(patchJsonPath);
            }
        }

        /// <summary>
        /// Read patch json
        /// </summary>
        public void ReadPatch(string patchJsonPath)
        {
            if (!File.Exists(patchJsonPath)) return;
            using FileStream fileStream = File.OpenRead(patchJsonPath);
            var dict = JsonSerializer.Deserialize<Dictionary<string, RszClassPatch>>(fileStream);
            if (dict == null) return;

            foreach (var item in dict)
            {
                ApplyPatch(item.Key, item.Value);
            }
        }

        public void ApplyPatch(string classname, RszClassPatch patch)
        {
            if (!classNameDict.TryGetValue(classname, out var rszClass)) return;

            if (!string.IsNullOrEmpty(patch.ReplaceName))
            {
                rszClass.name = patch.ReplaceName!;
            }
            if (patch.FieldPatches != null)
            {
                foreach (var fieldPatch in patch.FieldPatches)
                {
                    if (rszClass.GetField(fieldPatch.Name!) is RszField rszField)
                    {
                        if (!string.IsNullOrEmpty(fieldPatch.ReplaceName))
                        {
                            rszField.name = fieldPatch.ReplaceName!;
                        }
                        if (!string.IsNullOrEmpty(fieldPatch.OriginalType))
                        {
                            rszField.original_type = fieldPatch.OriginalType!;
                        }
                        if (fieldPatch.Type != RszFieldType.ukn_error && fieldPatch.Type != rszField.type)
                        {
                            rszField.type = fieldPatch.Type;
                            rszField.IsTypeInferred = true;
                        }
                    }
                }
            }
        }

        public string GetRSZClassName(uint classHash)
        {
            return classDict.TryGetValue(classHash, out var rszClass) ? rszClass.name : "Unknown Class!";
        }

        public RszClass? GetRSZClass(uint classHash)
        {
            return classDict.TryGetValue(classHash, out var rszClass) ? rszClass : null;
        }

        public RszClass? GetRSZClass(string className)
        {
            return classNameDict.TryGetValue(className, out var rszClass) ? rszClass : null;
        }

        public uint GetRSZClassCRC(uint classHash)
        {
            return classDict.TryGetValue(classHash, out var rszClass) ? rszClass.crc : 0;
        }

        public int GetFieldCount(uint classHash)
        {
            return classDict.TryGetValue(classHash, out var rszClass) ? rszClass.fields.Length : -1;
        }

        public RszField? GetField(uint classHash, uint fieldIndex)
        {
            if (classDict.TryGetValue(classHash, out var rszClass))
            {
                if (fieldIndex >= 0 && fieldIndex < rszClass.fields.Length)
                {
                    return rszClass.fields[fieldIndex];
                }
            }
            return null;
        }

        public int GetFieldAlignment(uint classHash, uint fieldIndex)
        {
            return GetField(classHash, fieldIndex)?.align ?? -1;
        }

        public bool GetFieldArrayState(uint classHash, uint fieldIndex)
        {
            return GetField(classHash, fieldIndex)?.array ?? false;
        }

        public string GetFieldName(uint classHash, uint fieldIndex)
        {
            return GetField(classHash, fieldIndex)?.name ?? "not found";
        }

        public string GetFieldTypeName(uint classHash, uint fieldIndex)
        {
            return GetField(classHash, fieldIndex)?.type.ToString() ?? "not found";
        }

        public string GetFieldOrgTypeName(uint classHash, uint fieldIndex)
        {
            return GetField(classHash, fieldIndex)?.original_type ?? "not found";
        }

        public int GetFieldSize(uint classHash, uint fieldIndex)
        {
            return GetField(classHash, fieldIndex)?.size ?? -1;
        }

        public RszFieldType GetFieldType(uint classHash, uint fieldIndex)
        {
            if (classDict.TryGetValue(classHash, out var rszClass))
            {
                if (fieldIndex >= 0 && fieldIndex < rszClass.fields.Length)
                {
                    return rszClass.fields[fieldIndex].type;
                }
                return RszFieldType.out_of_range;
            }
            return RszFieldType.class_not_found;
        }

        public bool IsClassNative(uint classHash)
        {
            return classDict.TryGetValue(classHash, out var rszClass) && rszClass.native;
        }

        public bool IsFieldNative(uint classHash, uint fieldIndex)
        {
            return GetField(classHash, fieldIndex)?.native ?? false;
        }
    }

    public class RszClass
    {
        [JsonIgnore]
        public uint typeId { get; set; }
        [JsonConverter(typeof(HexUIntJsonConverter))]
        public uint crc { get; set; }
        public string name { get; set; } = "";
        public bool native { get; set; }
        public RszField[] fields { get; set; } = [];

        /// <summary>
        /// Returns the plain class name without namespaces or parent classes.
        /// </summary>
        public string ShortName
        {
            get {
                int idx = name.LastIndexOf('.');
                return idx == -1 ? name : name.Substring(idx + 1);
            }
        }

        public static readonly RszClass Empty = new();

        public int IndexOfField(string name)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public RszField? GetField(string fieldName)
        {
            int index = IndexOfField(fieldName);
            if (index != -1) return fields[index];
            return null;
        }

        public override string ToString() => name;
    }

    public class RszField
    {
        public string name { get; set; } = "";
        public int align { get; set; }
        public int size { get; set; }
        public bool array { get; set; }
        public bool native { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter<RszFieldType>))]
        public RszFieldType type { get; set; }
        public string original_type { get; set; } = "";
        public RszClass? StructClass { get; set; }
        [JsonIgnore]
        public bool IsTypeInferred { get; set; }
        private string? displayType = null;

        public string? OriginalTypeOrNull => string.IsNullOrEmpty(original_type) ? null : original_type;

        public string DisplayType
        {
            get
            {
                if (displayType == null)
                {
                    if (string.IsNullOrEmpty(original_type))
                    {
                        displayType = array ? $"{type}[]" : type.ToString();
                        if (IsTypeInferred) displayType += "?";
                    }
                    else
                    {
                        displayType = original_type;
                    }
                }
                return displayType;
            }
        }

        public void GuessDataType()
        {
            if (type != RszFieldType.Data) return;
            type = size switch
            {
                1 => RszFieldType.U8,
                8 => align == 8 ? RszFieldType.U64 : RszFieldType.Vec2,
                16 => align == 8 ? RszFieldType.Guid : RszFieldType.Vec4,
                64 => RszFieldType.Mat4,
                80 => RszFieldType.OBB,
                _ => type
            };
            // IsTypeInferred = type != RszFieldType.Data;
        }

        public bool IsReference => type == RszFieldType.Object || type == RszFieldType.UserData;
        public bool IsString => type == RszFieldType.String || type == RszFieldType.Resource;

        public override string ToString() => $"{type} {name}";
    }


    /// <summary>
    /// rsz json patch
    /// </summary>
    public class RszClassPatch
    {
        public string? Name { get; set; }
        public string? ReplaceName { get; set; }
        public RszFieldPatch[]? FieldPatches { get; set; }

        public void AddFieldPatch(RszFieldPatch patch)
        {
            FieldPatches = FieldPatches == null ? [patch] : FieldPatches.Append(patch).ToArray();
        }
    }


    public class RszFieldPatch
    {
        public string? Name { get; set; }
        public string? ReplaceName { get; set; }
        public string? OriginalType { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter<RszFieldType>))]
        public RszFieldType Type { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter<KnownFileFormats>))]
        public KnownFileFormats FileFormat { get; set; }

        public override string ToString() => $"{Type} {Name} => {ReplaceName ?? Name}";
    }
}
