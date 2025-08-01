namespace ReeLib.Il2cpp;

using System.Numerics;
using System.Text.Json;
using ReeLib;

public abstract class EnumDescriptor
{
    public abstract Type BackingType { get; }

    public abstract IEnumerable<EnumCacheItem> CacheItems { get; }
    protected abstract IEnumerable<string> LabelValuePairs { get; }
    private string? _hintstring;
    public string HintstringLabels => _hintstring ??= string.Join(",", LabelValuePairs);

    public bool IsEmpty { get; private set; } = true;
    public bool IsFlags { get; set; }

    public abstract string GetLabel(object value);
    public abstract string GetLabel(JsonElement value);
    public abstract string[] GetLabels();
    public abstract object[] GetValues();

    public abstract bool HasFlag(object number, object flag);
    public abstract object AddFlag(object number, object flag);
    public abstract object RemoveFlag(object number, object flag);

    public void ParseIl2cppData(ObjectDef item)
    {
        if (item.fields == null) return;

        foreach (var (name, field) in item.fields.OrderBy(f => f.Value.Id)) {
            if (!field.Flags.Contains("SpecialName") && field.IsStatic && field.Default is JsonElement elem && elem.ValueKind == JsonValueKind.Number) {
                AddValue(name, elem);
            }
        }

        IsEmpty = false;
        IsFlags = GuessIsFlags();
    }

    public void ParseCacheData(IEnumerable<EnumCacheItem> pairs)
    {
        foreach (var item in pairs) {
            AddValue(item.name, item.value);
        }

        IsEmpty = false;
    }

    public void ParseCacheData(SignedEnumItem[] pairs)
    {
        foreach (var item in pairs) {
            AddValue(item.Name, JsonSerializer.SerializeToElement(item.Value));
        }

        IsEmpty = false;
    }

    protected abstract bool GuessIsFlags();
    public abstract void AddValue(string name, JsonElement elem);
}

public sealed class EnumDescriptor<T> : EnumDescriptor where T : struct, IBinaryInteger<T>, IBitwiseOperators<T, T, T>
{
    public readonly Dictionary<T, string> ValueToLabels = new();
    private readonly List<KeyValuePair<string, T>> OrderedValues = new();

    // need to use Convert.ChangeType because we aren't always getting the same type (e.g. rsz says uint, enum say int)
    public override bool HasFlag(object number, object flag) => ((T)Convert.ChangeType(number, typeof(T)) & (T)flag) != T.Zero;
    public override object AddFlag(object number, object flag) => Convert.ChangeType( ((T)Convert.ChangeType(number, typeof(T)) | (T)flag), number.GetType());
    public override object RemoveFlag(object number, object flag) => Convert.ChangeType( ((T)Convert.ChangeType(number, typeof(T)) & (~(T)flag)), number.GetType());

    public override Type BackingType => typeof(T);
    public override IEnumerable<EnumCacheItem> CacheItems => ValueToLabels
        .Select((pair) => new EnumCacheItem(pair.Value, JsonSerializer.SerializeToElement(pair.Key)));

    public static readonly EnumDescriptor<T> Default = new();
    private static readonly object DefaultValue = default(T);

    public override string GetLabel(object value) => ValueToLabels.TryGetValue((T)value, out var val) ? val : string.Empty;
    public override string GetLabel(JsonElement value) => GetLabel(Converter(value));

    private string[]? _labelsArray;
    public override string[] GetLabels()
    {
        if (_labelsArray == null) {
            _labelsArray = OrderedValues.Select(kv => kv.Key).ToArray();
        }
        return _labelsArray;
    }

    private object[]? _valuesArray;
    public override object[] GetValues()
    {
        if (_valuesArray == null) {
            _valuesArray = OrderedValues.Select(kv => (object)kv.Value).ToArray();
        }
        return _valuesArray;
    }

    private static Func<JsonElement, T>? converter;
    private static Func<JsonElement, T> Converter {
        get {
            if (converter == null) CreateConverter();
            return converter!;
        }
    }

    protected override IEnumerable<string> LabelValuePairs => ValueToLabels.Select((pair) => $"{pair.Value.Replace(":", "-")}:{pair.Key}");

    protected override bool GuessIsFlags()
    {
        if (!ValueToLabels.ContainsKey(T.One) || !ValueToLabels.ContainsKey(T.One + T.One)) {
            return false;
        }
        var values = ValueToLabels.Keys.Order().ToArray();
        T previous = values.First();
        var isSequential = true;
        foreach (var v in values.Skip(1)) {
            if (v == -T.One) continue;
            if (v == previous + T.One) {
                previous = v;
            } else {
                isSequential = false;
                break;
            }
        }
        if (isSequential) return false;
        var pots = values.Where(v => T.IsPow2(v));
        var maxPot = pots.LastOrDefault();
        if (maxPot == T.Zero) return false;
        var all = default(T);
        foreach (var pot in pots) all += pot;

        var potCount = pots.Count();
        var max = values.Last();
        if (max > all) return false;

        foreach (var nonpot in values.Where(n => !T.IsPow2(n))) {
            if ((nonpot & ~all) != T.Zero) return false;
        }
        return max != maxPot && potCount >= 2 || potCount >= 3;
    }

    public override void AddValue(string name, JsonElement elem)
    {
        if (converter == null) {
            CreateConverter();
        }
        T val = converter!(elem);
        if (!ValueToLabels.TryAdd(val, name)) {
            return;
        }
        OrderedValues.Add(new KeyValuePair<string, T>(name, val));
        _labelsArray = null;
        _valuesArray = null;
    }

    private static void CreateConverter()
    {
        // nasty; maybe add individual enum descriptor types eventually
        if (typeof(T) == typeof(System.Int64)) {
            converter = static (e) => {
                // need to handle both cases - raw il2cpp dump always prints as unsigned, whereas we're storing them with correct sign in the cache
                try {
                    return (T)(object)(long)e.GetInt64();
                } catch {
                    return (T)(object)(long)e.GetUInt64();
                }
            };
        } else if (typeof(T) == typeof(System.UInt64)) {
            converter = static (e) => (T)(object)e.GetUInt64();
        } else if (typeof(T) == typeof(System.Int32)) {
            converter = static (e) => (T)(object)(int)e.GetInt64();
        } else if (typeof(T) == typeof(System.UInt32)) {
            converter = static (e) => (T)(object)e.GetUInt32();
        } else if (typeof(T) == typeof(System.Int16)) {
            converter = static (e) => (T)(object)(short)e.GetInt32();
        } else if (typeof(T) == typeof(System.UInt16)) {
            converter = static (e) => (T)(object)e.GetUInt16();
        } else if (typeof(T) == typeof(System.SByte)) {
            converter = static (e) => (T)(object)(sbyte)e.GetInt32();
        } else if (typeof(T) == typeof(System.Byte)) {
            converter = static (e) => (T)(object)e.GetByte();
        } else {
            converter = static (e) => default(T);
        }
    }
}
