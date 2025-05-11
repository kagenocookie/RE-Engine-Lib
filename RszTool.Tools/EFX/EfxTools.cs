using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using RszTool.Common;
using RszTool.Efx;
using RszTool.Efx.Structs.RE4;
using RszTool.Efx.Structs.RERT;
using RszTool.InternalAttributes;

namespace RszTool.Tools;

public static class EfxTools
{
    private static readonly (string name, Type type)[] EmptyFields = Array.Empty<(string name, Type type)>();

    public static IEnumerable<(string name, Type type)> GetFieldInfo<T>(EfxVersion version) where T : EFXAttribute
        => GetFieldInfo(typeof(T), version);

    private static readonly Type[] VersionedArgs = [typeof(EfxVersion)];
    private static readonly Type[] NoArgs = Array.Empty<Type>();

    public static IEnumerable<(string name, Type type)> GetFieldInfo(Type targetType, EfxVersion version)
    {
        var method = targetType.GetMethod(nameof(EFXAttributeSpawn.GetFieldList), BindingFlags.Public|BindingFlags.Static, VersionedArgs);
        if (method == null) {
            method = targetType.GetMethod(nameof(EFXAttributeSpawn.GetFieldList), BindingFlags.Public|BindingFlags.Static, NoArgs);
            if (method != null) return method.Invoke(null, []) as IEnumerable<(string name, Type type)> ?? EmptyFields;
            return EmptyFields;
        }

        return method.Invoke(null, [version]) as IEnumerable<(string name, Type type)> ?? EmptyFields;
    }

    public static void GenerateEFXStructsJson(string outputFile, EfxVersion efxVersion)
    {
        if (efxVersion == EfxVersion.Unknown) return;

        Console.WriteLine($"Generating {efxVersion} EFX structs to {outputFile} ...");

        var asm = typeof(EFXAttributeSpawn).Assembly;

        var output = new EfxStructCache();
        var unhandledReferencedTypes = new HashSet<Type>();

        var efxTypes = EfxAttributeTypeRemapper.GetAllTypes(efxVersion);
        foreach (var (id, efxType) in efxTypes) {
            var instanceType = EfxAttributeTypeRemapper.GetAttributeInstanceType(efxType, efxVersion);
            if (instanceType == null) continue;

            var info = new EfxStructInfo() {
                TypeID = id,
                Name = efxType.ToString(),
                Classname = instanceType.FullName!,
            };
            info.Hash = (uint)GetStableStringHashCode(info.Classname);
            GenerateStructFields(instanceType, info, efxVersion, unhandledReferencedTypes);

            output.AttributeTypes[efxType.ToString()] = info;
        }

        var handledReferencedTypes = new HashSet<Type>();

        while (unhandledReferencedTypes.Count > 0) {
            var unhandled = new HashSet<Type>();
            foreach (var type in unhandledReferencedTypes) {
                if (!handledReferencedTypes.Add(type)) continue;

                if (type.IsEnum) {
                    var names = Enum.GetNames(type);

                    output.Enums.Add(
                        type.FullName!,
                        new SignedEnumData(
                            names.Select(name => new SignedEnumItem(name,
                                Convert.ToInt64(Enum.Parse(type, name, true))
                            )).ToArray(),
                            false,
                            type.GetEnumUnderlyingType().FullName!
                        ));
                } else if (type.IsClass) {
                    var info = new EfxStructInfo() {
                        Classname = type.FullName!,
                    };
                    info.Hash = (uint)GetStableStringHashCode(info.Classname);
                    GenerateStructFields(type, info, efxVersion, unhandled);
                    output.Structs.Add(info.Classname, info);
                }
                else // structs
                {
                    var structInfo = new EfxStructInfo() {
                        Classname = type.FullName!,
                    };
                    structInfo.Hash = (uint)GetStableStringHashCode(structInfo.Classname);
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                        var info = new EfxFieldInfo() {
                            Name = field.Name,
                            Flag = EfxFieldFlags.None,
                            FlagTarget = null,
                        };
                        // GenerateStructFields(field.FieldType, structInfo, efxVersion, unhandled);
                        MapEfxFieldType(field.FieldType, info, unhandled, field);
                        ExtendHash(structInfo, info);
                        structInfo.Fields.Add(info);
                    }
                    output.Structs.Add(structInfo.Classname, structInfo);
                }
            }
            unhandledReferencedTypes = unhandled;
        }

        var json = JsonSerializer.Serialize(output, GetJsonOptions());
        File.WriteAllText(outputFile, json);

        Console.WriteLine($"Done.");
    }

    private static void GenerateStructFields(Type instanceType, EfxStructInfo structInfo, EfxVersion efxVersion, HashSet<Type> referencedTypes)
    {
        var fields = EfxTools.GetFieldInfo(instanceType, efxVersion).ToList();
        foreach (var field in fields) {
            var fieldInfo = instanceType.GetField(field.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null) continue;

            var info = new EfxFieldInfo() {
                Name = field.name,
                Flag = EfxFieldFlags.None,
                FlagTarget = null,
            };
            MapEfxFieldType(field.type, info, referencedTypes, fieldInfo);

            if (fieldInfo.GetCustomAttribute<RszSwitchAttribute>() is RszSwitchAttribute switchAttr) {
                foreach (var subtype in switchAttr.Args.OfType<Type>()) {
                    referencedTypes.Add(subtype);
                }
            }

            ExtendHash(structInfo, info);
            structInfo.Fields.Add(info);
        }
    }

    private static JsonSerializerOptions GetJsonOptions()
    {
        if (jsonOptions != null) return jsonOptions;

        jsonOptions = new JsonSerializerOptions() {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        jsonOptions.Converters.Add(new JsonStringEnumConverter<RszFieldType>());
        jsonOptions.Converters.Add(new JsonStringEnumConverter<EfxFieldFlags>());
        return jsonOptions;
    }
    private static JsonSerializerOptions? jsonOptions;

    private static void ExtendHash(EfxStructInfo structInfo, EfxFieldInfo field)
    {
        structInfo.Hash = (uint)HashCode.Combine(
            structInfo.Hash,
            field.FieldType,
            field.IsArray,
            field.Flag,
            field.FlagTarget,
            GetStableStringHashCode(field.Name),
            GetStableStringHashCode(field.Classname ?? string.Empty));
    }

    private static int GetStableStringHashCode(string str)
    {
        // https://stackoverflow.com/a/36845864/4721768
        unchecked {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length && str[i] != '\0'; i += 2) {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    private static void MapEfxFieldType(Type type, EfxFieldInfo info, HashSet<Type> referencedTypes, FieldInfo? fieldInfo)
    {
        if (type.IsArray) {
            info.IsArray = true;
            type = type.GetElementType()!;
        } else if (type.IsGenericType && (
            type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            || type.GetGenericTypeDefinition() == typeof(List<>)
        )) {
            info.IsArray = true;
            type = type.GetGenericArguments()[0];
        }

        if (fieldInfo != null) {
            if (fieldInfo.GetCustomAttribute<RszArraySizeFieldAttribute>() is RszArraySizeFieldAttribute attr1) {
                info.Flag = EfxFieldFlags.StructSize;
                info.FlagTarget = attr1.ArrayField;
            } else if (fieldInfo.GetCustomAttribute<RszStringAsciiHashAttribute>() is RszStringAsciiHashAttribute attr2) {
                info.Flag = EfxFieldFlags.AsciiStringHash;
                info.FlagTarget = attr2.HashedStringField;
            } else if (fieldInfo.GetCustomAttribute<RszStringHashAttribute>() is RszStringHashAttribute attr3) {
                info.Flag = EfxFieldFlags.UTF16StringHash;
                info.FlagTarget = attr3.HashedStringField;
            } else if (fieldInfo.GetCustomAttribute<RszStringUTF8HashAttribute>() is RszStringUTF8HashAttribute attr4) {
                info.Flag = EfxFieldFlags.UTF8StringHash;
                info.FlagTarget = attr4.HashedStringField;
            } else if (fieldInfo.GetCustomAttribute<RszStringLengthFieldAttribute>() is RszStringLengthFieldAttribute attr5) {
                info.Flag = EfxFieldFlags.StringLength;
                info.FlagTarget = attr5.StringField;
            }
        }

        if (type == typeof(string)) {
            info.FieldType = RszFieldType.String;
            return;
        }

        if (type.IsClass) {
            info.FieldType = RszFieldType.Object;
            info.Classname = type.FullName;
            referencedTypes.Add(type);
            return;
        }

        if (type.IsEnum) {
            info.FieldType = RszInstance.CSharpTypeToRszFieldType(type.GetEnumUnderlyingType());
            info.Classname = type.FullName;
            referencedTypes.Add(type);
            return;
        }

        if (type == typeof(UndeterminedFieldType)) {
            info.FieldType = RszFieldType.U32;
            return;
        }

        info.FieldType = RszInstance.CSharpTypeToRszFieldType(type);

        if (info.FieldType == RszFieldType.ukn_error) {
            info.FieldType = RszFieldType.Struct;
            info.Classname = type.FullName;
            referencedTypes.Add(type);
        }
    }

}