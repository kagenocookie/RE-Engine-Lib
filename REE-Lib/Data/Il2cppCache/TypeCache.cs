namespace ReeLib.Il2cpp;

using System;
using System.Text.Json;

internal sealed class TypeCacheData
{
    public static readonly int CurrentCacheVersion = 2;

    public int CacheVersion { get; set; }
    public Dictionary<string, EnumCacheEntry> Enums { get; set; } = new();
    public Dictionary<string, List<string>> Subclasses { get; set; } = new();
}

public class EnumCacheEntry
{
    public bool IsFlags { get; set; }
    public string BackingType { get; set; } = string.Empty;
    public IEnumerable<EnumCacheItem> Items { get; set; } = Array.Empty<EnumCacheItem>();
}

public class TypePatch
{
    public bool IsFlagEnum { get; set; }
}

public record EnumCacheItem(string name, JsonElement value);

public class TypeCache
{
    public readonly Dictionary<string, EnumDescriptor> enums = new();
    public Dictionary<string, List<string>> subclasses = new();

    private static Dictionary<string, Func<EnumDescriptor>> descriptorFactory = new();

    private static readonly HashSet<string> ignoredBaseTypes = new() {
        "System.Object", "System.ValueType", "System.MulticastDelegate", "System.Delegate", "System.Array"
    };

    public void ApplyIl2cppData(Il2cppDump data)
    {
        enums.Clear();
        foreach (var (name, enumData) in data) {
            if (enumData.parent == null) continue;

            if (enumData.parent == "System.Enum") {
                var backing = GetEnumBackingType(enumData);
                if (backing == null) {
                    Console.Error.WriteLine("Couldn't determine enum backing type: " + name);
                    enums[name] = EnumDescriptor<int>.Default;
                    continue;
                }

                var descriptor = CreateEnumDescriptor(backing.FullName!);
                descriptor?.ParseIl2cppData(enumData);
                enums[name] = descriptor ?? EnumDescriptor<ulong>.Default;
            } else if (!name.Contains('!') && !ignoredBaseTypes.Contains(name) && !ignoredBaseTypes.Contains(enumData.parent) && !name.StartsWith("System.")) {
                var item = enumData;
                var parentName = name;

                // save only instantiable subclasses, ignore abstract / interfaces
                if (item.IsAbstract) continue;

                do {
                    if (item.parent == null) break;
                    if (!subclasses.TryGetValue(item.parent, out var list)) {
                        subclasses[item.parent] = list = new();
                        if (!data[item.parent].IsAbstract) {
                            // non-abstract base types should also be saved as instantiable
                            list.Add(item.parent);
                        }
                    }

                    list.Add(name);
                }
                while (data.TryGetValue(item.parent, out item) && item.parent != null && !ignoredBaseTypes.Contains(item.parent));
            }
        }
    }

    public bool ApplyPatches(Dictionary<string, TypePatch> patches)
    {
        var changed = false;
        foreach (var (cls, patch) in patches) {
            if (patch.IsFlagEnum && enums.TryGetValue(cls, out var enumDesc) && !enumDesc.IsFlags) {
                enumDesc.IsFlags = true;
                changed = true;
            }
        }
        return changed;
    }

    public List<string> GetSubclasses(string baseclass)
    {
        if (subclasses.TryGetValue(baseclass, out var list)) {
            return list;
        }

        return subclasses[baseclass] = new List<string>() { baseclass };
    }

    public bool IsAssignableTo(string classname, string baseClassname)
    {
        return classname == baseClassname || GetSubclasses(baseClassname).Contains(classname);
    }

    public EnumDescriptor GetEnumDescriptor(string classname)
    {
        if (enums.TryGetValue(classname, out var descriptor)) {
            return descriptor;
        }

        return EnumDescriptor<int>.Default;
    }

    internal void ApplyCacheData(TypeCacheData data)
    {
        foreach (var (enumName, enumItem) in data.Enums) {
            if (!enums.TryGetValue(enumName, out var enumInstance)) {
                var descriptor = CreateEnumDescriptor(enumItem.BackingType);
                if (descriptor != null) {
                    descriptor.ParseCacheData(enumItem.Items);
                    descriptor.IsFlags = enumItem.IsFlags;
                }
                enums[enumName] = descriptor ?? EnumDescriptor<ulong>.Default;
            }
        }
        subclasses = data.Subclasses;
    }

    internal TypeCacheData ToCacheData()
    {
        var data = new TypeCacheData() { CacheVersion = TypeCacheData.CurrentCacheVersion };
        foreach (var (name, entry) in enums) {
            var cacheEntry = new EnumCacheEntry();
            cacheEntry.BackingType = entry.BackingType.FullName!;
            cacheEntry.Items = entry.CacheItems;
            cacheEntry.IsFlags = entry.IsFlags;
            data.Enums.Add(name, cacheEntry);
        }
        data.Subclasses = subclasses;
        return data;
    }

    private static Type? GetEnumBackingType(ObjectDef item)
    {
        if (item.fields == null) {
            return null;
        }

        foreach (var (fieldName, field) in item.fields) {
            if (fieldName == "value__") { // !IsStatic instead?
                return Type.GetType(field.Type);
            }
        }

        return typeof(int);
    }

    private static EnumDescriptor? CreateEnumDescriptor(string backing)
    {
        if (!descriptorFactory.TryGetValue(backing, out var factory)) {
            var t = Type.GetType(backing);
            if (t == null) {
                Console.Error.WriteLine("Invalid cached enum backing type: " + backing);
                return null;
            }

            var enumType = typeof(EnumDescriptor<>).MakeGenericType(t!);
            descriptorFactory[backing] = factory = () => (EnumDescriptor)Activator.CreateInstance(enumType)!;
        }

        return factory();
    }

    private static readonly Dictionary<string, KnownFileFormats> resourceNameEnumMap = new() {
        { "MeshMaterial", KnownFileFormats.MaterialDefinition },
        { "RenderTargetTexture", KnownFileFormats.RenderTexture },
        { "FSMv2Tree", KnownFileFormats.Fsm2 },
        { "AIMapBase", KnownFileFormats.AIMap },
        { "AIMapNavMesh", KnownFileFormats.AIMap },
        { "AIMapWaypoint", KnownFileFormats.AIMap },
        { "AIMapWaypointSectionManager", KnownFileFormats.AIMap },
        { "AIMapVolumeSpace", KnownFileFormats.AIMap },
        { "Mixer", KnownFileFormats.AudioMixer },
        { "Definition", KnownFileFormats.DynamicsDefinition },
        { "Bank", KnownFileFormats.SoundBank },
        { "Package", KnownFileFormats.SoundPackage },
        { "JointExprGraph", KnownFileFormats.JointExpressionGraph },
    };
    private static readonly int initialResourceNameMapsCount = resourceNameEnumMap.Count;

    private static readonly Dictionary<KnownFileFormats, string> formatToResourceHolderMap = new();

    public static KnownFileFormats GetResourceFormat(string classname)
    {
        if (classname.EndsWith("ResourceHolder")) {
            if (resourceNameEnumMap.Count == initialResourceNameMapsCount) {
                SetupResourceHolderMapping();
            }
            var cls = classname[(classname.LastIndexOf('.') + 1)..^("ResourceHolder".Length)];
            if (resourceNameEnumMap.TryGetValue(cls, out var fmt)) {
                return fmt;
            }
        }
        return KnownFileFormats.Unknown;
    }

    public static string? GetResourceHolderClassname(KnownFileFormats format)
    {
        if (format == KnownFileFormats.Unknown) return null;

        if (resourceNameEnumMap.Count == initialResourceNameMapsCount) {
            SetupResourceHolderMapping();
        }
        return null;
    }

    private static void SetupResourceHolderMapping()
    {
        lock (resourceNameEnumMap) {
            var used = resourceNameEnumMap.Values.ToHashSet();
            if (resourceNameEnumMap.Count == initialResourceNameMapsCount) {
                foreach (var val in Enum.GetValues<KnownFileFormats>()) {
                    if (val == KnownFileFormats.Unknown || used.Contains(val)) continue;

                    var name = ((KnownFileFormats)val).ToString();
                    resourceNameEnumMap[name] = val;
                    formatToResourceHolderMap[val] = name;
                }
            } else Thread.Sleep(10);
        }
    }

    public KnownFileFormats[] GetResourceSubtypes(KnownFileFormats format)
    {
        return format switch {
            KnownFileFormats.MotionBase => [ KnownFileFormats.Motion, KnownFileFormats.MotionList, KnownFileFormats.GpuMotionList, KnownFileFormats.MotionCamera, KnownFileFormats.MotionCameraList ],
            KnownFileFormats.TimelineBase => [ KnownFileFormats.Timeline, KnownFileFormats.Clip ],
            KnownFileFormats.DynamicsBase => [ KnownFileFormats.RigidBodyMesh, KnownFileFormats.HeightField ],
            KnownFileFormats.BehaviorTreeBase => [ KnownFileFormats.BehaviorTree, KnownFileFormats.Fsm2 ],
            _ => []
        };
    }
}

