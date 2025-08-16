using ReeLib.Data;
using ReeLib.Efx;
using ReeLib.Il2cpp;

namespace ReeLib;

public sealed partial class Workspace : IDisposable
{
    // TODO MHR:pre-SB handling?
    public EfxVersion EfxVersion => config.Game.ToEfxVersion();

    private EfxCacheData LoadEfxDataCache()
    {
        var structCache = EfxTools.GenerateEFXStructsJson(config.Game.ToEfxVersion(), null);
        _efxCache = ReadEfxCache(structCache);
        return _efxCache ??= new();
    }

    private static EfxCacheData? ReadEfxCache(EfxStructCache cache)
    {
        EfxCacheData? data;
        data = new EfxCacheData();

        foreach (var ee in cache.Enums) {
            var backing = Type.GetType(ee.Value.BackingType)!;
            var enumType = typeof(EnumDescriptor<>).MakeGenericType(backing);
            var descriptor = (EnumDescriptor)Activator.CreateInstance(enumType)!;
            descriptor.ParseCacheData(ee.Value.Items);
            data.Enums.Add(ee.Key, descriptor);
        }

        foreach (var (name, attr) in cache.AttributeTypes) {
            EfxClassInfo info = BuildEfxClassInfo(attr, data);
            data.AttributeTypes[name] = info;
            data.Structs.Add(attr.Classname, info);
        }

        foreach (var (name, obj) in cache.Structs) {
            EfxClassInfo info = BuildEfxClassInfo(obj, data);
            data.Structs.Add(obj.Classname, info);
        }
        return data;
    }

    private static EfxClassInfo BuildEfxClassInfo(EfxStructInfo attr, EfxCacheData data)
    {
        var fieldDict = new Dictionary<string, EfxFieldInfo>();
        var info = new EfxClassInfo() {
            Fields = fieldDict,
            Info = attr,
            HasVersionedConstructor =
                !string.IsNullOrEmpty(attr.Classname) &&
                typeof(EFXAttribute).Assembly.GetType(attr.Classname)?.GetConstructor([typeof(EfxVersion)]) != null,
        };

        foreach (var field in attr.Fields) {
            if (field.Flag == EfxFieldFlags.BitSet) {
                fieldDict.Add(field.Name, field);
                continue;
            }
            if (field.Flag != EfxFieldFlags.None) continue;

            // RszFieldType.ukn_type is UndeterminedFieldType fields
            // they're mostly 0 in known available files, meaning they're likely meaningless for editing purposes
            if (field.FieldType == RszFieldType.ukn_type) continue;

            fieldDict.Add(field.Name, field);
        }
        return info;
    }

}