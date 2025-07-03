using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReeLib.Efx;
using ReeLib.Efx.Structs.Basic;
using ReeLib.InternalAttributes;

namespace ReeLib.Tools;

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
            if (instanceType == typeof(EFXAttributePlayEmitter)) {
                info.Fields.Last().Flag = EfxFieldFlags.StructSize;
                info.Fields.Last().FlagTarget = nameof(EFXAttributePlayEmitter.efxrData);
                info.Fields.Add(new EfxFieldInfo() {
                    Classname = typeof(EfxFile).FullName,
                    FieldType = RszFieldType.Data,
                    Name = nameof(EFXAttributePlayEmitter.efxrData),
                    Flag = EfxFieldFlags.EmbeddedEFX,
                    IsArray = true,
                });
                ExtendHash(info, info.Fields.Last());
            }

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
                        MapEfxFieldType(field.FieldType, info, unhandled, field, type);
                        ExtendHash(structInfo, info);
                        structInfo.Fields.Add(info);
                    }
                    output.Structs.Add(structInfo.Classname, structInfo);
                }
            }
            unhandledReferencedTypes = unhandled;
        }

        var json = JsonSerializer.Serialize(output, GetJsonOptions());
        Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
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
            MapEfxFieldType(field.type, info, referencedTypes, fieldInfo, instanceType);

            if (fieldInfo.GetCustomAttribute<RszFixedSizeArrayAttribute>() is RszFixedSizeArrayAttribute arraySize && arraySize.SizeFunc.Length == 0) {
                structInfo.Fields.Add(new EfxFieldInfo() {
                    Name = $"len_{field.name}",
                    Flag = EfxFieldFlags.StructSize,
                    FlagTarget = field.name,
                    FieldType = RszFieldType.S32,
                });
            }

            if (fieldInfo.GetCustomAttribute<RszListAttribute>() is RszListAttribute listSize && listSize.SizeFunc.Length == 0) {
                structInfo.Fields.Add(new EfxFieldInfo() {
                    Name = $"len_{field.name}",
                    Flag = EfxFieldFlags.StructSize,
                    FlagTarget = field.name,
                    FieldType = RszFieldType.S32,
                });
            }

            if (fieldInfo.GetCustomAttribute<RszSwitchAttribute>() is RszSwitchAttribute switchAttr) {
                foreach (var subtype in switchAttr.Args.OfType<Type>()) {
                    referencedTypes.Add(subtype);
                }
            }

            if (fieldInfo.GetCustomAttribute<RszByteSizeFieldAttribute>() is RszByteSizeFieldAttribute sizeAttr) {
                info.Flag = EfxFieldFlags.StructSize;
                info.FlagTarget = sizeAttr.TargetObject;
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
        structInfo.Hash = structInfo.Hash * 31 + (uint)field.FieldType;
        structInfo.Hash = structInfo.Hash * 31 + (field.IsArray ? 1u : 0u);
        structInfo.Hash = structInfo.Hash * 31 + (uint)field.Flag;
        structInfo.Hash = structInfo.Hash * 31 + (uint)GetStableStringHashCode(field.FlagTarget);
        structInfo.Hash = structInfo.Hash * 31 + (uint)GetStableStringHashCode(field.Name);
        structInfo.Hash = structInfo.Hash * 31 + (uint)GetStableStringHashCode(field.Classname);
    }

    private static int GetStableStringHashCode(string? str)
    {
        if (str == null) return 0;
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

    private static void MapEfxFieldType(Type type, EfxFieldInfo info, HashSet<Type> referencedTypes, FieldInfo? fieldInfo, Type ownerType)
    {
        if (type.IsArray) {
            info.IsArray = true;
            type = type.GetElementType()!;
            if (fieldInfo != null && fieldInfo.GetCustomAttribute<RszFixedSizeArrayAttribute>() is RszFixedSizeArrayAttribute fixedSize && fixedSize.SizeFunc.Length == 1 && fixedSize.SizeFunc.All(n => n is int)) {
                info.FixedLength = (int)fixedSize.SizeFunc[0];
            }
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

        if (type == typeof(BitSet)) {
            info.FieldType = RszFieldType.U32;
            info.IsArray = true;
            info.Flag = EfxFieldFlags.BitSet;
            if (fieldInfo != null) {
                var inst = Activator.CreateInstance(ownerType);
                var defaultValue = (BitSet?)fieldInfo.GetValue(inst);
                if (defaultValue != null) {
                    info.FixedLength = defaultValue.Bits.Length;
                    info.FlagTarget = defaultValue.BitCount.ToString();
                }
            }
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
            info.FieldType = RszFieldType.ukn_type;
            return;
        }

        info.FieldType = RszInstance.CSharpTypeToRszFieldType(type);

        if (info.FieldType == RszFieldType.ukn_error) {
            info.FieldType = RszFieldType.Struct;
            info.Classname = type.FullName;
            referencedTypes.Add(type);
        }
    }

    public static IEnumerable<EFXAttribute> GetAttributesAndActions(this EfxFile file, bool includeEmbeddedEfx = false)
    {
        foreach (var e in file.Entries) {
            foreach (var a in e.Attributes) yield return a;
        }
        foreach (var e in file.Actions) {
            foreach (var a in e.Attributes) {
                yield return a;
                if (includeEmbeddedEfx && a is EFXAttributePlayEmitter emitter && emitter.efxrData != null) {
                    foreach (var child in emitter.efxrData.GetAttributesAndActions(includeEmbeddedEfx)) {
                        yield return child;
                    }
                }
            }
        }
    }

    public static IEnumerable<EfxFile> GetEmbeddedFiles(this EfxFile file)
    {
        yield return file;
        foreach (var e in file.Actions) {
            foreach (var a in e.Attributes) {
                if (a is EFXAttributePlayEmitter emitter && emitter.efxrData != null) {
                    yield return emitter.efxrData;
                }
            }
        }
    }
}