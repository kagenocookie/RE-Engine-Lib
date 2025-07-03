namespace ReeLib.Data;

using System.Reflection;
using ReeLib.Efx;
using ReeLib.Il2cpp;

public class EfxClassInfo
{
    public EfxStructInfo Info { get; set; } = null!;
    public Dictionary<string, EfxFieldInfo> Fields { get; set; } = new();
    public bool HasVersionedConstructor { get; set; }

    private FieldInfo[]? _fieldInfos;
    public FieldInfo[] FieldInfos => _fieldInfos ??= GetFieldInfos();

    private FieldInfo[] GetFieldInfos()
    {
        var arr = new FieldInfo[Fields.Count];

        var targetType = typeof(EFXAttribute).Assembly.GetType(Info.Classname)
            ?? throw new Exception("Invalid efx target type " + Info.Classname);

        int i = 0;
        foreach (var f in Fields) {
            arr[i++] = targetType.GetField(f.Key, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic)
                ?? throw new Exception("Invalid efx field " + f.Value.Name);
        }

        return arr;
    }
}

public sealed class EfxCacheData
{
    public Dictionary<string, EfxClassInfo> AttributeTypes = new();
    public Dictionary<string, EfxClassInfo> Structs = new();
    public Dictionary<string, EnumDescriptor> Enums = new();
}

public static class EfxExtensions
{
    public static GameIdentifier GetGameIdentifier(this EfxVersion version)
    {
        return version switch {
            EfxVersion.DD2 => GameIdentifier.dd2,
            EfxVersion.DMC5 => GameIdentifier.dmc5,
            EfxVersion.MHRise => GameIdentifier.mhrise,
            EfxVersion.MHRiseSB => GameIdentifier.mhrise,
            EfxVersion.MHWilds => GameIdentifier.mhwilds,
            EfxVersion.RE2 => GameIdentifier.re2,
            EfxVersion.RE3 => GameIdentifier.re3,
            EfxVersion.RE4 => GameIdentifier.re4,
            EfxVersion.RE7 => GameIdentifier.re7,
            EfxVersion.RE8 => GameIdentifier.re8,
            EfxVersion.SF6 => GameIdentifier.sf6,
            EfxVersion.RERT => GameIdentifier.re2rt,
            _ => GameIdentifier.Unknown,
        };
    }

    public static EfxVersion ToEfxVersion(this GameIdentifier game)
    {
        return game.hash switch {
            GameNameHash.dd2 => EfxVersion.DD2,
            GameNameHash.dmc5 => EfxVersion.DMC5,
            GameNameHash.mhrise => EfxVersion.MHRiseSB,
            GameNameHash.mhwilds => EfxVersion.MHWilds,
            GameNameHash.re2 => EfxVersion.RE2,
            GameNameHash.re3 => EfxVersion.RE3,
            GameNameHash.re4 => EfxVersion.RE4,
            GameNameHash.re7 => EfxVersion.RE7,
            GameNameHash.re8 => EfxVersion.RE8,
            GameNameHash.sf6 => EfxVersion.SF6,
            GameNameHash.re2rt => EfxVersion.RERT,
            GameNameHash.re3rt => EfxVersion.RERT,
            GameNameHash.re7rt => EfxVersion.RERT,
            _ => EfxVersion.Unknown,
        };
    }
}